using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace Common {
    public class ColorTool {
        // TODO
        private static Dictionary<int, Color> _colors = new Dictionary<int, Color> {
                { -1, Color.clear},
                { 0, Color.white},
                { 1, new Color(0xaf,0xa3,0xdb)},
                { 2, new Color(0x99,0xab,0xdd)},
                { 3, new Color(0x65,0xbf,0xdf)},
                { 4, new Color(0xaa,0xd4,0xb6)},
                { 5, new Color(0xd3,0xbc,0xaa)},
                { 6, new Color(0xd2,0xb4,0xaa)},
                { 7, new Color(0xe8,0xa5,0xa4)}
        };

        public static Color GetColor(int colorid) {
            return _colors[colorid];
        }
    }
}