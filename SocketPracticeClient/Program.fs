open System
open System.Net
open System.Net.Sockets
open System.Text


let localip = IPAddress.Parse "192.168.1.69"

let BootClient =
  let clientSocket = new Socket(SocketType.Stream, ProtocolType.Tcp)
  printfn "clientsocket has been initialized"
  clientSocket//clientsocket is connected

let rec TryConnect (clientSocket : Socket) (connectIP : IPAddress) =
  if clientSocket.Poll(1000, SelectMode.SelectWrite) || 1=1 then
    try
      printfn "clientsocket is attempting connection..."
      clientSocket.Connect(IPEndPoint(connectIP, 8888))
    with
      | err -> printfn "an error ocured, this one: %A" err
  else
    printfn "no connection available"
  clientSocket

let SendData (clientSocket:Socket) =
  printfn "trying to send data, input some shit please"
  let dataToSend = Console.ReadLine()
  let dataAsBytes = Encoding.ASCII.GetBytes(dataToSend)
  let _ = clientSocket.Send(dataAsBytes)
  ()

let rec TrySendData (clientSocket:Socket) =
  if clientSocket.Poll(1000, SelectMode.SelectWrite) then
    SendData clientSocket
  TrySendData clientSocket
  ()

//THE ACTUAL PROGRAM
let clientSocket = BootClient

let rec MainClientLoop (clientSocket:Socket) =
  let clientSocket' = TryConnect clientSocket localip
  if clientSocket'.Poll(1000, SelectMode.SelectWrite) then
    TrySendData clientSocket'
  MainClientLoop clientSocket'

do MainClientLoop clientSocket