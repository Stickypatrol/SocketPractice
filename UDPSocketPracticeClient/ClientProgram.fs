open System
open System.Text
open System.Net
open System.Net.Sockets
open System.Threading

type Settings =
  {
    LocalIP : IPAddress
    LocalPort : int
  }

let localSettings = {LocalIP = IPAddress.Parse "145.24.244.103"; LocalPort = Int32.Parse(Console.ReadLine())}

let BootClient (settings : Settings) : Socket =
  printfn "initializing serversocket..."
  let clientSocket = new Socket(SocketType.Dgram, ProtocolType.Udp)
  printfn "correctly initialized..."
  clientSocket

let SendData (socket:Socket) (sett:Settings) =
  let buffer = Encoding.ASCII.GetBytes(Console.ReadLine())
  ignore <| socket.SendTo(buffer, IPEndPoint(sett.LocalIP, 8888))

//ACTUAL PROGRAM

let socket = BootClient localSettings

let rec MainLoop (socket:Socket) (settings:Settings) =
  do SendData socket settings
  MainLoop socket settings

do MainLoop socket localSettings