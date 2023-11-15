using DogScepterLib.Core.Chunks;
using DogScepterLib.Core.Models;
using DogScepterLib.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Linq;

namespace DogScepterLib.Project.Assets
{
    public class AssetScript : Asset
    {
        public string Code { get; set; }
        public bool Constructor { get; set; }

        public new static Asset Load(string assetScript)
        {
            byte[] buff = File.ReadAllBytes(assetScript);
            var res = JsonSerializer.Deserialize<AssetScript>(buff, ProjectFile.JsonOptions);
            ComputeHash(res, buff);
            return res;
        }

        protected override byte[] WriteInternal(ProjectFile pf, string assetPath, bool actuallyWrite)
        {
            byte[] buff = JsonSerializer.SerializeToUtf8Bytes(this, ProjectFile.JsonOptions);
            if (actuallyWrite)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(assetPath));
                using (FileStream fs = new FileStream(assetPath, FileMode.Create))
                    fs.Write(buff, 0, buff.Length);

                if (Code != null)
                {
                    int assetIndex = pf.Code.FindIndex(Code);
                    if (assetIndex != -1)
                    {
                        pf.AddAssetsToJSON(pf.Code, new List<int>() { assetIndex }, true,
                            Path.GetRelativePath(pf.DirectoryPath, Path.GetDirectoryName(assetPath)).Replace('\\', '/'));
                    }
                }
            }
            return buff;
        }

        public override void Delete(string assetPath)
        {
            if (File.Exists(assetPath))
                File.Delete(assetPath);
        }
    }
}
