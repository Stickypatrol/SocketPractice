open System
open System.Net
open System.Net.Sockets
open System.Text
open Newtonsoft.Json

let localip = IPAddress.Parse "145.24.221.121"

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
  let dataToSend = [1;2;3],[4;5;6]
  let x = JsonConvert.SerializeObject(dataToSend)
  let dataAsBytes = Encoding.ASCII.GetBytes(x)
  let _ = clientSocket.Send(dataAsBytes)
  ignore <| Console.ReadLine()
  ()

let TryReceiveData (clientSocket:Socket) =
  printfn "trying to receive response..."
  let buffer = Array.create 50 (new Byte())
  let _ = clientSocket.Receive(buffer)
  let x = JsonConvert.DeserializeObject<List<float>*List<float>>(Encoding.ASCII.GetString(buffer))
  printfn "Data received is: %A" x
  List.iter2 (fun x y -> printfn "item in the list is %A and %A" x y) (fst x) (snd x)
  ()

let rec TrySendData (clientSocket:Socket) =
  if clientSocket.Poll(1000, SelectMode.SelectWrite) then
    SendData clientSocket
    TryReceiveData clientSocket
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