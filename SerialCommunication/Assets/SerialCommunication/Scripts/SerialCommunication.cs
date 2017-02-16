using UnityEngine;
using System.Collections;

public class SerialCommunication : MonoBehaviour
{
    public SerialHandler serialHandler;
    private bool serial_sendable = true;
    private float timeElapsed, timeArduino;
    private int v0 = 0, v1 = 0, v2 = 0;

    void Start ()
    {
        serialHandler.OnDataReceived += OnDataReceived;
    }
	
	void Update () {

    }

    void FixedUpdate()
    {
        timeElapsed += Time.deltaTime;

        if (serial_sendable || timeElapsed > 0.5)
        {
            serial_sendable = false;
            timeElapsed = 0;
            v0 = 0;
            v1 = 1;
            v2 = 2;
            serialHandler.Write("h" + v0.ToString() + "," + v1.ToString() + "," + v2.ToString() + "\0");
        }
    }

    void OnDataReceived(string message)
    {
        var data = message.Split(new string[] { "," }, System.StringSplitOptions.None);
        serial_sendable = true;
        // if (data.Length != 2) return;

        float tmp = timeArduino;
        if (!float.TryParse(data[0], out tmp))
        {
            return;
        }
        timeArduino = tmp;

        /*
        try
        {
            // Debug.Log(data.Length.ToString() + ", " + data[0] + ", " + data[1]);
            // ard_time = float.Parse(data[0])/1000;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e.Message);
        }
        */
    }
}
