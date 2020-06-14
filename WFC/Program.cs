using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace WFC {
    class Program {
        private static IGrid _grid;
        private static Dictionary<uint, string> _presetsMap = new Dictionary<uint, string>();
        private static Preset _preset;
        private static WFCModified _wfc;

        static void Main(string[] args) {
            string presetsPath = @"Presets";
            Console.WriteLine($"Scanning {presetsPath} directory for preset files...\n");

            string[] filePaths = Directory.GetFiles(presetsPath, "*.xml", SearchOption.TopDirectoryOnly);
            for (uint index = 0; index < filePaths.Length; ++index) {
                string presetName = Path.GetFileNameWithoutExtension(filePaths[index]);
                Console.WriteLine($"Preset Id: {index}, Preset name: `{presetName}`");
                _presetsMap.Add(index, presetName);
            }

            int presetId = -1;
            while (presetId < 0 || presetId >= _presetsMap.Count) {
                Console.Write("Choose a preset by an id: ");
                presetId = Convert.ToInt32(Console.ReadLine());
            }

            Console.WriteLine($"Parsing `{_presetsMap[(uint)presetId]}` preset...");
            _preset = new Preset(_presetsMap[(uint)presetId]);
            int voxelSize = _preset.Parse();

            _grid = new UniformGrid(3, 4, 2, voxelSize); // Hardcode grid size for now..
            Console.WriteLine($"Created a uniform grid of {_grid.GetCountX()}x{_grid.GetCountY()}x{_grid.GetCountZ()} dimensions");

            _wfc = new WFCModified(_grid, _preset);
            _wfc.Run();

            _grid.ToModel(_preset);
        }
    }
}
