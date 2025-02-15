using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace Maze
{
    public class Maze : MonoBehaviour
    {
        public List<Room> rooms=new List<Room>();
        public List<Connection> connections = new List<Connection>();

        public void AddRoom(Room room)
        {
            rooms.Add(room);
        }

        public void ConnectRooms(Room A, Room B, Connection.connections Acon, Connection.connections Bcon)
        {
            Connection conn = new Connection();
            conn.A = A; conn.B=B;
            conn.conA = Acon;
            conn.conB = Bcon;
            A.connections[(int)conn.conA] = conn;
            B.connections[(int)conn.conB] = conn;
        }

        public bool CheckIntegrity(bool verbose=false)
        {
            foreach (Room room in rooms)
            {
                for(int i = 0; i<room.connections.Length; i++)
                {
                    if (room.connections[i] == null) continue;
                    //Verifica que la conexion origen y destino están bien seteadas en ambas salas
                    Connection connection = room.connections[i];
                    if(connection.A == room)
                    {
                        //Verifica que el arco de conexión tiene bien esatblecido el origen
                        if (connection.conA != (Connection.connections)i)
                            Debug.LogWarning("INCONSISTENCIA: room: "+room.name + " arco "+ (Connection.connections)i+ " mal en origen A");

                        //Verifica que la room destination del arco tiene el arco en conB
                        if(connection.B.connections[(int)connection.conB] != connection)
							Debug.LogWarning("INCONSISTENCIA: room: " + connection.B.name + " arco " + connection.conB + " mal en origen destino B");
					    if(verbose)
                        {
                            Debug.Log("SUCCESFULL Room :" + room.name + " connects from " + (Connection.connections)i + " to " + connection.B.name + " arriving at " + connection.conB); 
                        }
                    }
					else
                    {
						//Verifica que el arco de conexión tiene bien esatblecido el origen
						if (connection.conB != (Connection.connections)i)
							Debug.LogWarning("INCONSISTENCIA: room: " + room.name + " arco " + (Connection.connections)i + " mal en destino B");

						//Verifica que la room destination del arco tiene el arco en conB
						if (connection.A.connections[(int)connection.conA] != connection)
							Debug.LogWarning("INCONSISTENCIA: room: " + connection.A.name + " arco " + connection.conA + " mal en origen origen A");

						if (verbose)
						{
							Debug.Log("SUCCESFULL Room :" + room.name + " connects from " + (Connection.connections)i + " to " + connection.A.name + " arriving at " + connection.conA);
						}
					}
                }
            }
            return true;
        }

        
    }
}