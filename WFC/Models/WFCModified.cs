using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace WFC {
    interface IWFC {    // In case different implementation will be required or for another dimension
        public bool Run();
    }

    class WFCModified : IWFC {
        private IGrid               _grid;
        private Preset              _preset;
        private int                 _capacity;
        private List<int>[]         _cells;
        private bool[]              _visited;
        //private float[]             _weights;
        private int[]               _entropies;
        private Random              _random;
        private Tuple<int, int>     _currentCell;

        public WFCModified(IGrid grid, Preset preset) {
            _grid = grid;
            _preset = preset;

            _capacity   = _grid.GetCountX() * _grid.GetCountY() * _grid.GetCountZ();
            _visited    = new bool[_capacity];
            _entropies  = new int[_capacity];
            _cells      = new List<int>[_capacity];
            //_weights    = new float[_preset.Models.Count];

            for (int i = 0; i < _capacity; ++i) {
                _cells[i] = new List<int>();
            }

            //for (int i = 0; i < _preset.Models.Count; ++i) {
            //    _weights[i] = 1.0f;
            //}

            for (int i = 0; i < _capacity; ++i) {
                _entropies[i] = _preset.Models.Count;
            }
        }

        //private double FindEntropy() {
        //    double sumOfWeights = 0;
        //    double sumOfWeightLogWeights = 0;

        //    for (int t = 0; t < _weights.Length; t++) {
        //        sumOfWeights += _weights[t];
        //        sumOfWeightLogWeights += _weights[t] * Math.Log(_weights[t]);
        //    }

        //    return Math.Log(sumOfWeights) - sumOfWeightLogWeights / sumOfWeights;
        //}

        private bool Observe() {
            int min = _preset.Proxies.Count + 1;
            int argmin = -1;

            for (int i = 0; i < _cells.Length; i++) {
                if (_visited[i]) continue;

                int entropy = _entropies[i];
                if (entropy < min) {
                    min = entropy;
                    argmin = i;
                }
            }

            if (argmin == -1) {
                return true;
            }

            _visited[argmin] = true;

            // TODO: Fix this shitty workaround
            if (_cells[argmin].Count < 1) {
                int randomIndex = _random.Next(0, _preset.Models.Count - 1);
                _currentCell = Tuple.Create(argmin, randomIndex);
            }
            else {
                int randomIndex = _random.Next(0, _cells[argmin].Count - 1);
                _currentCell = Tuple.Create(argmin, _cells[argmin][randomIndex]);
            }

            _grid.GetCells()[argmin] = _currentCell.Item2;
            _entropies[argmin] = 1;

            return false;
        }

        private void Propagate() {
            var coords = _grid.Coords(_currentCell.Item1);
            int coordX = coords.Item1;
            int coordY = coords.Item2;
            int coordZ = coords.Item3;

            int leftX   = coordX - 1;
            int rightX  = coordX + 1;
            int bottomY = coordY - 1;
            int topY    = coordY + 1;
            int backZ   = coordZ - 1;
            int frontZ  = coordZ + 1;

            int leftCoord   = _grid.Index(leftX, coordY, coordZ);
            int rightCoord  = _grid.Index(rightX, coordY, coordZ);
            int bottomCoord = _grid.Index(coordX, bottomY, coordZ);
            int topCoord    = _grid.Index(coordX, topY, coordZ);
            int backCoord   = _grid.Index(coordX, coordY, backZ);
            int frontCoord  = _grid.Index(coordX, coordY, frontZ);

            int modelId = _currentCell.Item2;
            UpdateNeighbour(leftCoord, _preset.Proxies[modelId].LeftNeighbours);
            UpdateNeighbour(rightCoord, _preset.Proxies[modelId].RightNeighbours);
            UpdateNeighbour(bottomCoord, _preset.Proxies[modelId].BottomNeighbours);
            UpdateNeighbour(topCoord, _preset.Proxies[modelId].TopNeighbours);
            UpdateNeighbour(backCoord, _preset.Proxies[modelId].BackNeighbours);
            UpdateNeighbour(frontCoord, _preset.Proxies[modelId].FrontNeighbours);
        }

        public bool Run() {
            //var startingEntropy = FindEntropy();
            Random random = new Random();
            _random = new Random(random.Next());

            while(true) {
                if (Observe()) {
                    return true;
                }

                Propagate();
            }
        }

        private void UpdateNeighbour(int index, List<int> neighbours) {
            int argmin = _currentCell.Item1;
            if (_grid.IsValid(index) && !_visited[index]) {
                for (int i = 0; i < neighbours.Count; i++) {
                    if (!_cells[index].Contains(i))
                        _cells[index].Add(neighbours[i]);
                }
                _entropies[index] = _cells[index].Count;
            }
        }
    }
}
