using System;
using System.Collections.Generic;
using System.Text;

namespace WFC {
    enum NeighbourAxis {
        Left,
        Right,
        Top,
        Bottom,
        Front,
        Back
    }

    struct ModelProxy {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<int> LeftNeighbours { get; set; }
        public List<int> RightNeighbours { get; set; }
        public List<int> FrontNeighbours { get; set; }
        public List<int> BackNeighbours { get; set; }
        public List<int> TopNeighbours { get; set; }
        public List<int> BottomNeighbours { get; set; }

        public ModelProxy(int id, string name) {
            Id = id;
            Name = name;
            LeftNeighbours  = new List<int>();
            RightNeighbours = new List<int>();
            FrontNeighbours = new List<int>();
            BackNeighbours  = new List<int>();
            TopNeighbours   = new List<int>();
            BottomNeighbours = new List<int>();
        }
    }

    class Preset {
        private PresetParser _parser;
        public string Name { get; private set; }
        public List<Model> Models { get; set; }
        public List<ModelProxy> Proxies { get; private set; }

        public Preset(string name) {
            Name = name;
            Models = new List<Model>();
            Proxies = new List<ModelProxy>();
            _parser = new PresetParser($"{name}.xml");
        }

        public int Parse() {
            int vSize = _parser.Parse(this);
            foreach (var model in Models) {
                model.Import();
            }

            return vSize;
        }
    }
}
