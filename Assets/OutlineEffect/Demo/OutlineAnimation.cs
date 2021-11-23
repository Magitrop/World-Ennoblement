using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

namespace cakeslice
{
    public class OutlineAnimation : MonoBehaviour
    {
        bool pingPong = false;
        void Update()
        {
            if (GetComponent<OutlineEffect>().lineColors.Length > 0)
            {
                Color c = GetComponent<OutlineEffect>().lineColors[0];

                if (pingPong)
                {
                    c.a += Time.deltaTime;

                    if (c.a >= 1)
                        pingPong = false;
                }
                else
                {
                    c.a -= Time.deltaTime;

                    if (c.a <= 0)
                        pingPong = true;
                }

                c.a = Mathf.Clamp01(c.a);
                GetComponent<OutlineEffect>().lineColors[0] = c;
                GetComponent<OutlineEffect>().UpdateMaterialsPublicProperties();
            }
        }
    }
}