using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

[ExecuteAlways]
public class MapSetterScript : MonoBehaviour
{
    public int Size = 20;
    public int Height = 3;
    private Transform Ground;
    private Transform[] Walls;
    private Transform Roof;

    public float BaseY { get; private set; } = -.5f;

    private void Start()
    {
        Assert.IsTrue(Size > 10);
        Assert.IsTrue(transform.childCount == 6);

        Ground = transform.GetChild(0);
        Walls = Enumerable.Range(0, 4).Select(i => transform.GetChild(i + 1)).ToArray();
        Roof = transform.GetChild(5);

        Ground.transform.localPosition = Vector3.up * BaseY;
        Ground.transform.localScale = new Vector3(Size, .05f, Size);

        Roof.transform.localPosition = Vector3.up * (BaseY + Height) / 2;
        Roof.transform.localScale = new Vector3(Size, .05f, Size);
        Roof.transform.localEulerAngles = Vector3.right * 180f;


        Walls[0].transform.localPosition = Vector3.forward * Size / 2;
        Walls[1].transform.localPosition = -Vector3.forward * Size / 2;
        Walls[0].transform.localRotation = Quaternion.identity;
        Walls[1].transform.localRotation = Quaternion.identity;

        Walls[2].transform.localPosition = Vector3.right * Size / 2;
        Walls[3].transform.localPosition = -Vector3.right * Size / 2;
        Walls[2].transform.localRotation = Quaternion.Euler(0, 90, 0);
        Walls[3].transform.localRotation = Quaternion.Euler(0, 90, 0);

        for (int i = 0; i < 4; i++)
        {
            Walls[i].localScale = new Vector3(Size, Height, 1);
            Walls[i].localPosition += Vector3.up * (BaseY + Height) / 2;
        }

        Debug.Log("Wall reloaded");
    }
}