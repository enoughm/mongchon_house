/*!
 * \file
 * \brief Get distance data from Ethernet type URG
 * \author Jun Fujimoto
 * $Id: get_distance_ethernet.cs 403 2013-07-11 05:24:12Z fujimoto $
 */
using UnityEngine;
using System.Threading;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;
using SCIP_library;

public class UrgDeviceEthernetCustom : UrgDevice
{
//	private Thread listenThread;
	private Thread clientThread;
	TcpClient _tcpClient;
	
	public List<long> distances;
	public List<long> strengths;

//	private Queue messageQueue;
	
	private string ip_address = "192.168.0.10";
	private int port_number = 10940;

    public Action<List<long>> onReadMD;
    public Action<List<long>, List<long>> onReadME;

    
    private bool _isConnected;
    private float _connectTryInterval = 10f;
    public Action onConnected;
    
    Thread _readThread;

    public void StartTCP(string ip = "192.168.0.10", int port = 10940, Action onConnected = null)
    {
	    StopAllCoroutines();

	    _isConnected = false;
	    
		ip_address = ip;
		port_number = port;
		this.onConnected = onConnected;

		distances = new List<long>();
		strengths = new List<long>();

		if (_readThread != null)
			_readThread.Abort();
		
		StartCoroutine(StartTryConnectLoop(_connectTryInterval));
    }

    private void ListenForClients()
    {
	    _readThread = new Thread(HandleClientComm);
	    _readThread.Start(_tcpClient);
    }

    void OnDisable()
    {
	    DeInit();
    }
    void OnApplicationQuit()
    {
	    DeInit();
    }
	
    void DeInit()
    {
	    if(_tcpClient != null){
		    if(_tcpClient.Connected ){
			    NetworkStream stream = _tcpClient.GetStream();
			    if(stream != null){
				    stream.Close();
			    }
		    }
		    _tcpClient.Close();
	    }
	    
	    _readThread?.Abort();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    IEnumerator StartTryConnectLoop(float interval)
    {
	    while (true)
	    {
		    if (!_isConnected)
		    {
			    try
			    {
				    _tcpClient?.Close();
			    } 		
			    catch (Exception e) { 	
				    Debug.LogError(e);
			    }
			    var t = Task.Run(ConnectToSensor);
			    yield return new WaitUntil(()=>t.IsCompleted);
			    ListenForClients();
			    Debug.Log($"ConnectToSensor Completed: _isConnected:{_isConnected}");
		    }
		    yield return new WaitForSeconds(interval);
	    }
    }

    private async Task ConnectToSensor()
    {
	    try {
		    if (_isConnected)
			    return;
		    
		    _isConnected = false;
		    _tcpClient = new TcpClient();
		    await _tcpClient.ConnectAsync(ip_address, port_number);
		    _isConnected = true;

		    Debug.Log("Connect setting = IP Address : " + ip_address + " Port number : " + port_number.ToString());
		    onConnected?.Invoke();
	    } catch (Exception ex) {
		    Debug.Log(ex.Message);
	    } finally {

	    }
    }
    
    
    public void Write(string scip)
    {
	    NetworkStream stream = _tcpClient.GetStream();
	    write(stream, scip);
    }
    

	private void HandleClientComm(object obj)
	{
		try
		{
			using (TcpClient client = (TcpClient)obj)
			{
				using (NetworkStream stream = client.GetStream())
				{
//					NetworkStream clientStream = client.GetStream();
					while (true)
					{
						long time_stamp = 0;
						string receive_data = read_line(stream);
//						messageQueue.Enqueue( receive_data );

						string cmd = GetCommand(receive_data);
						if(cmd == GetCMDString(CMD.MD)){
							distances.Clear();
							SCIP_Reader.MD(receive_data, ref time_stamp, ref distances);
                            if (onReadMD != null)
                                onReadMD.Invoke(distances);
						}else if(cmd == GetCMDString(CMD.ME)){
							distances.Clear();
							strengths.Clear();
							SCIP_Reader.ME(receive_data, ref time_stamp, ref distances, ref strengths);
                            if (onReadME != null)
                                onReadME.Invoke(distances, strengths);
						}else{
							Debug.Log(">>"+receive_data);
						}
					}
//					client.Close();
				}
			}
		} catch (System.Exception ex) {
			Debug.LogWarning("error: "+ex);
		}
	}

	string GetCommand(string get_command)
	{
		string[] split_command = get_command.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
		return split_command[0].Substring(0, 2);
	}
	
	bool CheckCommand(string get_command, string cmd)
	{
		string[] split_command = get_command.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
		return split_command[0].StartsWith(cmd);
	}


    /// <summary>
    /// Read to "\n\n" from NetworkStream
    /// </summary>
    /// <returns>receive data</returns>
    static string read_line(NetworkStream stream)
    {
        if (stream.CanRead) {
            StringBuilder sb = new StringBuilder();
            bool is_NL2 = false;
            bool is_NL = false;
            do {
                char buf = (char)stream.ReadByte();
                if (buf == '\n') {
                    if (is_NL) {
                        is_NL2 = true;
                    } else {
                        is_NL = true;
                    }
                } else {
                    is_NL = false;
                }
                sb.Append(buf);
            } while (!is_NL2);

            return sb.ToString();
        } else {
            return null;
        }
    }

    /// <summary>
    /// write data
    /// </summary>
    static bool write(NetworkStream stream, string data)
    {
        if (stream.CanWrite) {
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            stream.Write(buffer, 0, buffer.Length);
            return true;
        } else {
            return false;
        }
    }
}