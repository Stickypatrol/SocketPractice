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

let localSettings = {LocalIP = IPAddress.Parse "192.168.178.117"; LocalPort = 8888}

let BootProgram (settings : Settings) : Socket =
  printfn "initializing serversocket..."
  let serverSocket = new Socket(SocketType.Dgram, ProtocolType.Udp)
  printfn "correctly initialized..."
  serverSocket.Bind(IPEndPoint(settings.LocalIP, settings.LocalPort))
  printfn "socket is bound to port \"%A\"..." settings.LocalPort
  serverSocket

let ReceiveData (socket:Socket) =
  let buffer = Array.create 100 (new Byte())
  ignore <| socket.Receive(buffer)
  printfn "%s" (Encoding.ASCII.GetString(buffer))

//ACTUAL PROGRAM

let socket = BootProgram localSettings

let rec MainLoop (socket:Socket) =
  do ReceiveData socket
  MainLoop socket

do MainLoop socket






//snippet of code from another project
(*
let serviceClient (client: TcpClient) =
    let buffer = Array.create 1000 (new Byte())
    use stream = client.GetStream()
    stream.Write(buffer, 0, 1000)  // write header
    while true do
        client

let asyncServiceClient (client: TcpClient) = async {
    use stream = client.GetStream()
    do! stream.AsyncWrite(quote, 0, 1)  // write header
    while true do
        do! asyncWriteStockQuote(stream) }

async*)