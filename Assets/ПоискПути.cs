using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ПоискПути : MonoBehaviour
{
    public Transform СтартоваяТочка;
    public Transform КонечнаяТочка;
    List<Vector3> ТочкиПути = new List<Vector3>();
    public Transform prefab;
    void Start()
    {
        //Протестировать не на нулевых коооринатах
        float angle = 0;
        float Сторона = Vector3.Dot(СтартоваяТочка.right, КонечнаяТочка.position);
        if (Сторона > 0)
        {
            angle += Vector3.Angle(СтартоваяТочка.position + Vector3.forward, КонечнаяТочка.position);
        }
        else
        {
            angle -= Vector3.Angle(СтартоваяТочка.position + Vector3.forward, КонечнаяТочка.position);
        }
        float угол = 0.0174532862792735f * angle;
        for (float i = 0; i < 10; i++)
        {
            float x = КонечнаяТочка.position.x + 3 * Mathf.Sin(угол);
            float z = КонечнаяТочка.position.z - 3 * (1 - Mathf.Cos(угол));
            Instantiate(prefab, new Vector3(x, 0, z + 3), Quaternion.identity);
            угол += 0.1745328627927352f;
        }

        // ТочкиПути.Add(СтартоваяТочка.position);
        // StartCoroutine(ПоискСледующейТочки());
        // StartCoroutine(ОтрисовкаПути());
    }
    IEnumerator ПоискСледующейТочки()
    {
        while (true)
        {
            RaycastHit результатПопадения;
            Ray луч = new Ray(ТочкиПути[ТочкиПути.Count - 1], КонечнаяТочка.position - ТочкиПути[ТочкиПути.Count - 1]);
            if (Physics.Raycast(луч, out результатПопадения, 2))
            {
                while (true)
                {
                    // float x = результатПопадения.point.x + 2 * sin(5)
                    // float y = результатПопадения.point.y - 2 * (1 - cos(5))
                    // Vector3 НоваяТочка = ;
                    Ray луч2 = new Ray(ТочкиПути[ТочкиПути.Count - 1], КонечнаяТочка.position - ТочкиПути[ТочкиПути.Count - 1]);
                    if (Physics.Raycast(луч, out результатПопадения, 2))
                    {

                    }
                    yield return new WaitForSeconds(0.5f);
                }
                // Instantiate(prefab, НоваяТочка, Quaternion.identity);
            }
            else
            {
                if (Vector3.Distance(ТочкиПути[ТочкиПути.Count - 1], луч.GetPoint(2)) < Vector3.Distance(ТочкиПути[ТочкиПути.Count - 1], КонечнаяТочка.position))
                {
                    ТочкиПути.Add(луч.GetPoint(2));
                }
                else
                {
                    ТочкиПути.Add(КонечнаяТочка.position);
                    break;
                }
            }
            yield return new WaitForSeconds(1);
        }
    }
    IEnumerator ОтрисовкаПути()
    {
        while (true)
        {
            if (ТочкиПути.Count > 1)
            {
                for (int i = 0; i < ТочкиПути.Count - 1; i++)
                {
                    Debug.DrawRay(ТочкиПути[i], ТочкиПути[i + 1] - ТочкиПути[i], Color.red, 1);
                }
            }
            yield return new WaitForSeconds(1);
        }
    }
}