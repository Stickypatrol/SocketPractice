open System
open System.Text
open System.Net
open System.Net.Sockets
open Monad

type Settings =
  {
    LocalIP : IPAddress
    LocalPort : int
  }

type ProgramState =
  {
    LocalSettings : Settings
    ClientSockets : List<Socket>
  }

let localSettings = {LocalIP = IPAddress.Parse "192.168.1.69"; LocalPort = 8888}
//just hardcode the IP for now. we can figure out a proper solution in the future

let BootProgram (settings : Settings) =
  let serverSocket = new Socket(SocketType.Stream, ProtocolType.Tcp)
  printfn "Server has started..."
  serverSocket.Bind(IPEndPoint(settings.LocalIP, settings.LocalPort))
  printfn "socket is bound to IP \"%A\" and port \"%A\"..." settings.LocalIP settings.LocalPort
  serverSocket.Listen 60
  printfn "socket is listening for connections..."
  serverSocket

let rec connectClient (serverSocket:Socket) =
  if serverSocket.Poll(1000, SelectMode.SelectRead) then
    printfn "accepting client"
    (serverSocket.Accept())
  else
    connectClient serverSocket

let WriteSentData (socket:Socket) =
  let buffer = Array.create 50 (new Byte())
  let _ = socket.Receive(buffer)
  printfn "Data received is: %A" (Encoding.ASCII.GetString(buffer))
  let _ = (socket.Blocking = false)
  ()

let socket = BootProgram localSettings

let rec MainServerLoop (serverSocket : Socket) =
  if (serverSocket.Available > 0) then
    WriteSentData serverSocket
  MainServerLoop serverSocket

do MainServerLoop (connectClient socket)