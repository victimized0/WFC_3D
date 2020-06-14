using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace WFC {
    class Vector2 {
        public float X { get; set; }
        public float Y { get; set; }

        public Vector2(float x, float y) {
            X = x;
            Y = y;
        }
    }

    class Vector3 {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Vector3(float x, float y, float z) {
            X = x;
            Y = y;
            Z = z;
        }

        public static Vector3 operator +(Vector3 a, Vector3 b) {
            return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

    }

    struct CFace {
        public int[] VertexIndices { get; set; }
        public int[] NormalIndices { get; set; }
        public int[] TextureIndices { get; set; }

        public CFace(int[] vIndices, int[] nIndices, int[] tIndices) {
            VertexIndices = new int[vIndices.Length];
            NormalIndices = new int[nIndices.Length];
            TextureIndices = new int[tIndices.Length];

            vIndices.CopyTo(VertexIndices, 0);
            nIndices.CopyTo(NormalIndices, 0);
            tIndices.CopyTo(TextureIndices, 0);
        }
    }

    class Model {
        public List<Vector3> Vertices { get; set; }
        public List<Vector3> Normals { get; set; }
        public List<Vector2> TexCoords { get; set; }
        public List<CFace> Faces { get; set; }
        public IModelParser Parser { get; set; }

        public Model(string path) {
            Parser      = new ObjParser(path);   // Hardcode obj parser
            Vertices    = new List<Vector3>();
            Normals     = new List<Vector3>();
            TexCoords   = new List<Vector2>();
            Faces       = new List<CFace>();
        }

        public bool Import() {
            return Parser.Deserialise(this);
        }

        public bool Export(string outPath) {
            return Parser.Serialise(this, outPath);
        }

        public void Append(Model m, Vector3 pivot) {
            if (m.Vertices.Count < 3) return;

            int oldFacesPos = Faces.Count;
            Faces.AddRange(m.Faces);

            for (int i = oldFacesPos; i < Faces.Count; ++i) {
                for (int j = 0; j < Faces[i].VertexIndices.Length; ++j) {
                    Faces[i].VertexIndices[j] += Vertices.Count;
                }

                for (int j = 0; j < Faces[i].NormalIndices.Length; ++j) {
                    Faces[i].NormalIndices[j] += Normals.Count;
                }

                for (int j = 0; j < Faces[i].TextureIndices.Length; ++j) {
                    Faces[i].TextureIndices[j] += TexCoords.Count;
                }
            }

            Vertices.AddRange(m.Vertices);
            Normals.AddRange(m.Normals);
            TexCoords.AddRange(m.TexCoords);

            for (int i = 0; i < Vertices.Count; ++i) {
                Vertices[i] += pivot;
            }
        }
    }
}
