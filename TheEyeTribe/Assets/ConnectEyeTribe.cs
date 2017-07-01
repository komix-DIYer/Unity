/*
 * http://lab.aquacp.com/2014/01/21/unityeyetribe01/
 * 
 * Error
 * ・出力
 * 　- get_currentResolution can only be called from the main thread.
 * 　- get_transform can only be called from the main thread.
 * ・意味
 * 　別スレッドでは Unity の機能（get_currentResolution，get_transform）は使えない（Unity はシングルスレッド志向）
 * ・対処
 * 　- 画面解像度は Start 時に取得
 * 　- 位置の反映は Update で実施（位置のローカル変数 vec をメンバ変数に変更）
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using LitJson;
 
public class Point
{
    public double x { get; set; }
    public double y { get; set; }
}
public class Eye
{
    public Point raw { get; set; }
    public Point avg { get; set; }
    public Point pcenter { get; set; }
    public double psize { get; set; }
}
public class Frame
{
    public Point avg { get; set; }
    public bool fix { get; set; }
    public Eye lefteye { get; set; }
    public Eye righteye { get; set; }
    public Point raw { get; set; }
    public int state { get; set; }
    public int time { get; set; }
}
public class Track
{
    public Frame frame { get; set; }
}
public class Packet
{
    public string category { get; set; }
    public string request { get; set; }
    public int statuscode { get; set; }
    public Track values { get; set; }
}

public class ConnectEyeTribe : MonoBehaviour
{
    TcpClient socket;
    private Thread incomingThread;
    private bool isRunning = false;
    private System.Timers.Timer timerHeartbeat;
    private int CurrentResolutionHeight;
    private Vector3 vec;

    void Start()
    {
        //最大画面サイズに変更する
        Resolution[] resolutions = Screen.resolutions;
        int len = resolutions.Length - 1;
        Screen.SetResolution(resolutions[len].width, resolutions[len].height, true);

        try
        {
            socket = new TcpClient("localhost", 6555);
        }
        catch (Exception ex)
        {
            Debug.Log("Error connecting: " + ex.Message);
        }

        string REQ_CONNECT = "{\"values\":{\"push\":true,\"version\":1},\"category\":\"tracker\",\"request\":\"set\"}";
        Send(REQ_CONNECT);

        incomingThread = new Thread(ListenerLoop);
        incomingThread.Start();

        string REQ_HEATBEAT = "{\"category\":\"heartbeat\",\"request\":null}";
        timerHeartbeat = new System.Timers.Timer(300);
        timerHeartbeat.Elapsed += delegate { Send(REQ_HEATBEAT); };
        timerHeartbeat.Start();
        CurrentResolutionHeight = Screen.currentResolution.height;
    }
    private void Send(string message)
    {
        if (socket != null && socket.Connected)
        {
            StreamWriter writer = new StreamWriter(socket.GetStream());
            writer.WriteLine(message);
            writer.Flush();
        }
    }
    void Update()
    {
        if (Input.GetKey("escape")) Application.Quit();

        transform.position = Camera.main.ScreenToWorldPoint(vec);
    }
    private void ListenerLoop()
    {
        StreamReader reader = new StreamReader(socket.GetStream());
        isRunning = true;

        while (isRunning)
        {
            string response = string.Empty;
            try
            {
                response = reader.ReadLine();
                Packet packets = JsonMapper.ToObject<Packet>(response);
                double fx = packets.values.frame.avg.x;
                double fy = packets.values.frame.avg.y;
                // Vector3 vec = new Vector3((float)fx, Screen.currentResolution.height - (float)fy, 10f);
                vec = new Vector3((float)fx, CurrentResolutionHeight - (float)fy, 10f);
                // transform.position = Camera.main.ScreenToWorldPoint(vec);
            }
            catch (Exception ex)
            {
                Debug.Log("Error while reading response: " + ex.Message);
            }

        }
    }
}