using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperFunctions : MonoBehaviour {

    // ============================== CLOSEST NUMBER SLOT ==============================

    public static NumberSlot getClosestNumberSlot(Vector3 position, NumberSlotController numberSlotController) {
        int closestIndex;
        float closestDistance;
        return getClosestNumberSlot(position, numberSlotController, out closestIndex, out closestDistance);
    }

    public static NumberSlot getClosestNumberSlot(Vector3 position, NumberSlotController numberSlotController, out float closestDistance) {
        int closestIndex;
        return getClosestNumberSlot(position, numberSlotController, out closestIndex, out closestDistance);
    }

    public static NumberSlot getClosestNumberSlot(Vector3 position, NumberSlotController numberSlotController, out int closestIndex, out float closestDistance) {

        // Find the closest number slot
        closestIndex = -1;
        closestDistance = float.MaxValue;
        for (int i = 0; i < numberSlotController.numberSlots.Count; i++) {

            // If that slot already has a number in it, skip it
            if (numberSlotController.numberSlots[i].hasNumber) {
                continue;
            }

            // Check if this number slot is the currently closest one
            float distance = Vector3.Distance(position, numberSlotController.numberSlots[i].transform.position);
            if (distance < closestDistance) {
                closestDistance = distance;
                closestIndex = i;
            }

        }

        // If a valid number slot was found, return it
        if (closestIndex > -1) {
        return numberSlotController.numberSlots[closestIndex];
        }
        // Otherwise, return no number slot
        else {
            return null;
        }

    }

    // ============================== DEBUG TEXT ==============================

    // From: http://dinodini.weebly.com/debugdrawtext.html
    public static void DebugText(Vector3 p, string v, Color colour, float size = 1.0f, float duration = 0.0f) {

        /*
        *   a b
        *  cdefg
        *   h i
        *  jklmn
        *   o p
        *   
        *   bits: abcd efgh ijkl mnop
        */

        // Line segments
        Vector4[] lines = new Vector4[] {
            new Vector4(-1, 1, 0, 1),       // a
            new Vector4(0, 1, 1, 1),        // b
            new Vector4(-1, 1, -1, 0),      // c
            new Vector4(-1, 1, 0, 0),       // d
            new Vector4(0, 1, 0, 0),        // e
            new Vector4(1, 1, 0, 0),        // f
            new Vector4(1, 1, 1, 0),        // g
            new Vector4(-1, 0, 0, 0),       // h
            new Vector4(0, 0, 1, 0),        // i
            new Vector4(-1, 0, -1, -1),     // j
            new Vector4(0, 0, -1, 0 - 1),   // k
            new Vector4(0, 0, 0, -1),       // l
            new Vector4(0, 0, 1, -1),       // m
            new Vector4(1, 0, 1, -1),       // n
            new Vector4(-1, -1, 0, -1),     // o
            new Vector4(0, -1, 1, -1),      // p
        };

        // Character mappings
        int[] chars = {
            0xe667, // 0
            0x0604, // 1
            0xc3c3, // 2
            0xc287, // 3
            0x2990, // 4
            0xe187, // 5
            0xe1c7, // 6
            0xc410, // 7
            0xe3c7, // 8
            0xe384, // 9

            0xe3c4, // A
            0xca97, // B
            0xe043, // C
            0xca17, // D
            0xe1c3, // E
            0xe140, // F
            0xe0c7, // G
            0x23c4, // H
            0xc813, // I
            0xc852, // J
            0x2548, // K
            0x2043, // L
            0x3644, // M
            0x324c, // N
            0xe247, // O
            0xe3c0, // P
            0xe24f, // Q
            0xe3c8, // R
            0xd087, // S
            0xc810, // T
            0x2247, // U
            0x2460, // V
            0x226c, // W
            0x1428, // X
            0x1410, // Y
            0xc423, // Z

            0x0000, // space
            0x0002, // .
            0x0100, // -
            0x0420, // /
        };

        // Draw each character
        for (int m = 0; m < v.Length; m++) {
            int n = v[m];
            int c = -1;
            if (n >= '0' && n <= '9') {
                n = n - '0';
                c = chars[n];
            }
            else if (n >= 'A' && n <= 'Z') {
                n = n - 'A' + 10;
                c = chars[n];
            }
            else if (n >= 'a' && n <= 'z') {
                n = n - 'a' + 10;
                c = chars[n];
            }
            else if (n == ' ') {
                c = chars[26 + 10];
            }
            else if (n == '.') {
                c = chars[26 + 11];
            }
            else if (n == '-') {
                c = chars[26 + 12];
            }
            else if (n == '/') {
                c = chars[26 + 13];
            }
            for (int i = 0; i < 16; i++) {
                if ((c & (1 << (15 - i))) != 0) {
                    Debug.DrawLine(
                        new Vector3(m * 2.0f * size + p.x + lines[i].x / 2 * size, p.y, p.z + lines[i].y * size),
                        new Vector3(m * 2.0f * size + p.x + lines[i].z / 2 * size, p.y, p.z + lines[i].w * size),
                        colour,
                        duration);
                }
            }
        }
    }

}