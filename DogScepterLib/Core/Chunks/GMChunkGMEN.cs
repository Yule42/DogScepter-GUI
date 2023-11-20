using System;
using System.Collections.Generic;
using System.Text;

namespace DogScepterLib.Core.Chunks
{
    // Game end code entries (undocumented)
    /// <summary>
    /// Contains AtGameEnd scripts.
    /// </summary>
    // this chunk is a list of indexes of scripts executed using gml_pragma("AtGameEnd", "scr_scriptname()"); to run them after the GameEnd event.
    // heck if any game actually uses it
    public class GMChunkGMEN : GMChunk
    {
        public List<int> List;

        public override void Serialize(GMDataWriter writer)
        {
            base.Serialize(writer);

            writer.Write(List.Count);
            foreach (int item in List)
                writer.Write(item);
        }

        public override void Deserialize(GMDataReader reader)
        {
            base.Deserialize(reader);

            int count = reader.ReadInt32();
            List = new List<int>(count);
            for (int i = count; i > 0; i--)
                List.Add(reader.ReadInt32());
        }
    }
}
