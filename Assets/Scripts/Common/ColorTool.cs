using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace Common {
    public class ColorTool {
        // TODO
        private static readonly Dictionary<int, Color> _colors = new Dictionary<int, Color> {
                { -1, Color.clear},
                { 0, new Color32(0xff,0xff,0xff,0x50)},
                { 1, new Color32(0xaf,0xa3,0xdb,0xff)},
                { 2, new Color32(0x99,0xab,0xdd,0xff)},
                { 3, new Color32(0x65,0xbf,0xdf,0xff)},
                { 4, new Color32(0xaa,0xd4,0xb6,0xff)},
                { 5, new Color32(0xd3,0xbc,0xaa,0xff)},
                { 6, new Color32(0xdd,0xa9,0xa2,0xff)},
                { 7, new Color32(0xf0,0x9b,0x9a,0xff)}
        };

        public static Color GetColor(int colorid) {
            if (!_colors.ContainsKey(colorid)) {
                return Color.clear;
            } else {
                return _colors[colorid];
            }
        }
    }
}