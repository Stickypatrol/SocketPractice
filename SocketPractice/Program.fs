open System
open System.Text
open System.Net
open System.Net.Sockets
open Monad
open Newtonsoft.Json

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
    let client = (serverSocket.Accept())
    client, serverSocket
  else
    connectClient serverSocket

let WriteSentData (clientSocket:Socket) (serverSocket:Socket) =
  let buffer = Array.create 50 (new Byte())
  let _ = clientSocket.Receive(buffer)
  let x = JsonConvert.DeserializeObject<List<float>*List<float>>(Encoding.ASCII.GetString(buffer))
  printfn "Data received is: %A" x
  List.iter2 (fun x y -> printfn "item in the list is %A and %A" x y) (fst x) (snd x)
  ignore <| clientSocket.Send(buffer)
  let _ = (serverSocket.Blocking = false)
  ()

//ACTUAL PROGRAM

let settings = {LocalIP = (IPAddress.Parse "145.24.221.121"); LocalPort = 8888}

let socket = BootProgram settings

let rec MainServerLoop (clientSocket:Socket) (serverSocket : Socket) =
  if (clientSocket.Available > 0) then
    WriteSentData clientSocket serverSocket
  MainServerLoop clientSocket serverSocket


let client, server = connectClient socket
do MainServerLoop client server