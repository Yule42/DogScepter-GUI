using DogScepterLib.Core;
using DogScepterLib.Core.Chunks;
using DogScepterLib.Core.Models;
using DogScepterLib.Project.Assets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogScepterLib.Project.Converters
{
    public class ScriptConverter : AssetConverter<AssetScript>
    {
        public override void ConvertData(ProjectFile pf, int index)
        {
            var dataCode = ((GMChunkCODE)pf.DataHandle.Chunks["CODE"])?.List;

            GMScript asset = (GMScript)pf.Scripts[index].DataAsset;

            AssetScript projectAsset = new AssetScript()
            {
                Name = asset.Name?.Content,
                Code = (dataCode == null) ? asset.CodeID.ToString() : dataCode[asset.CodeID].Name.Content,
                Constructor = asset.Constructor
            };

            pf.Scripts[index].Asset = projectAsset;
        }

        public override void ConvertData(ProjectFile pf)
        {
            EmptyRefsForNamed(pf.DataHandle.GetChunk<GMChunkSCPT>().List, pf.Scripts);
        }

        public override void ConvertProject(ProjectFile pf)
        {
            GMList<GMScript> dataAssets = pf.DataHandle.GetChunk<GMChunkSCPT>().List;

            GMList<GMCode> dataCode = ((GMChunkCODE)pf.DataHandle.Chunks["CODE"])?.List;

            int getCode(string name)
            {
                if (dataCode == null)
                    return -1;
                try
                {
                    return dataCode.Select((elem, index) => new { elem, index }).First(p => p.elem.Name.Content == name).index;
                }
                catch (InvalidOperationException)
                {
                    return -1;
                }
            }

            dataAssets.Clear();
            for (int i = 0; i < pf.Scripts.Count; i++)
            {
                // Assign new data index to this asset ref
                pf.Scripts[i].DataIndex = dataAssets.Count;

                // Get project-level asset
                AssetScript assetScript = pf.Scripts[i].Asset;
                if (assetScript == null)
                {
                    // This asset was never converted
                    dataAssets.Add((GMScript)pf.Scripts[i].DataAsset);
                    continue;
                }

                dataAssets.Add(new GMScript()
                {
                    Name = pf.DataHandle.DefineString(assetScript.Name),
                    CodeID = getCode(assetScript.Code),
                    Constructor = assetScript.Constructor
                });
            }
        }
    }
}
