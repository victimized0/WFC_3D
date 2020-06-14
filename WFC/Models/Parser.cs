using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ObjParser;
using ObjParser.Types;

namespace WFC {
    interface IModelParser {
        public bool Serialise(Model model, string outPath);
        public bool Deserialise(Model model);
    }

    class ObjParser : IModelParser {
        private string _filePath;
        public string OutPath { get; set; }

        public ObjParser(string path) {
            _filePath = path;
        }

        public bool Serialise(Model model, string outPath) {
            Obj obj = new Obj();

            for (int index = 0; index < model.Vertices.Count; ++index) {
                double x = model.Vertices[index].X;
                double y = model.Vertices[index].Y;
                double z = model.Vertices[index].Z;
                obj.VertexList.Add(new Vertex(x, y, z));
            }

            for (int index = 0; index < model.Normals.Count; ++index) {
                double x = model.Normals[index].X;
                double y = model.Normals[index].Y;
                double z = model.Normals[index].Z;
                obj.NormalList.Add(new Normal(x, y, z));
            }

            for (int index = 0; index < model.TexCoords.Count; ++index) {
                double x = model.TexCoords[index].X;
                double y = model.TexCoords[index].Y;
                obj.TextureList.Add(new TextureVertex(x, y));
            }

            for (int index = 0; index < model.Faces.Count; ++index) {
                int[] vInds = model.Faces[index].VertexIndices;
                int[] nInds = model.Faces[index].NormalIndices;
                int[] tInds = model.Faces[index].TextureIndices;
                obj.FaceList.Add(new Face(vInds, nInds, tInds));
            }

            obj.WriteObjFile(outPath, null);
            return true;
        }

        public bool Deserialise(Model model) {
            if (_filePath == "") return true;
            string execPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            string fullpath = @$"{execPath}..\..\..\Presets\{_filePath}";

            Obj obj = new Obj();
            obj.LoadObj(fullpath);

            for (int index = 0; index < obj.VertexList.Count; ++index) {
                float x = (float)obj.VertexList[index].X;
                float y = (float)obj.VertexList[index].Y;
                float z = (float)obj.VertexList[index].Z;
                model.Vertices.Add(new Vector3(x, y, z));
            }

            for (int index = 0; index < obj.NormalList.Count; ++index) {
                float x = (float)obj.NormalList[index].X;
                float y = (float)obj.NormalList[index].Y;
                float z = (float)obj.NormalList[index].Z;
                model.Normals.Add(new Vector3(x, y, z));
            }

            for (int index = 0; index < obj.TextureList.Count; ++index) {
                float x = (float)obj.TextureList[index].X;
                float y = (float)obj.TextureList[index].Y;
                model.TexCoords.Add(new Vector2(x, y));
            }

            for (int index = 0; index < obj.FaceList.Count; ++index) {
                int[] vInds = obj.FaceList[index].VertexIndexList;
                int[] nInds = obj.FaceList[index].NormalIndexList;
                int[] tInds = obj.FaceList[index].TextureVertexIndexList;
                model.Faces.Add(new CFace(vInds, nInds, tInds));
            }

            return true;
        }
    }

    class PresetParser {
        private string _path;

        public PresetParser(string path) {
            _path = path;
        }

        public int Parse(Preset preset) {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(@$"Presets\{_path}");

            XmlNode root = xmlDoc.SelectSingleNode("preset");
            int voxelSize = 0;
            if (!int.TryParse(root.Attributes["voxel_size"].Value, out voxelSize)) {
                return -1;
            }

            var models = xmlDoc.SelectNodes("//preset/models/model");
            int index = 0;
            foreach (XmlNode model in models) {
                var name = model.Attributes["name"].Value;
                var modelPath = model.Attributes["path"].Value;
                var newModel = new Model(modelPath);
                preset.Models.Add(newModel);
                preset.Proxies.Add(new ModelProxy(index++, name));
            }

            var adjacentModels = xmlDoc.SelectNodes("//preset/neighbours/model");
            foreach (XmlNode model in adjacentModels) {
                var modelName = model.Attributes["name"].Value;
                int sampleId = 0;
                for (; sampleId < preset.Proxies.Count; ++sampleId) {
                    if (preset.Proxies[sampleId].Name == modelName) {
                        break;
                    }
                }

                foreach (XmlNode neighbour in model.ChildNodes) {
                    var neighbourName = neighbour.Attributes["name"].Value;
                    var neighbourAxis = Enum.Parse(typeof(NeighbourAxis), neighbour.Attributes["axis"].Value);

                    index = 0;
                    for (; index < preset.Proxies.Count; ++index) {
                        if (preset.Proxies[index].Name == neighbourName) {
                            break;
                        }
                    }

                    switch (neighbourAxis) {
                        case NeighbourAxis.Left:
                            preset.Proxies[sampleId].LeftNeighbours.Add(index);
                            break;
                        case NeighbourAxis.Right:
                            preset.Proxies[sampleId].RightNeighbours.Add(index);
                            break;
                        case NeighbourAxis.Top:
                            preset.Proxies[sampleId].TopNeighbours.Add(index);
                            break;
                        case NeighbourAxis.Bottom:
                            preset.Proxies[sampleId].BottomNeighbours.Add(index);
                            break;
                        case NeighbourAxis.Front:
                            preset.Proxies[sampleId].FrontNeighbours.Add(index);
                            break;
                        case NeighbourAxis.Back:
                            preset.Proxies[sampleId].BackNeighbours.Add(index);
                            break;
                        default: break;
                    }

                    //preset.Samples.Add(new ModelPreset(index));
                }
            }

            return voxelSize;
        }
    }
}
