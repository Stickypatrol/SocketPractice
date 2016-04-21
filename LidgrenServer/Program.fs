
open Lidgren.Network

let config = new NetPeerConfiguration("LidgrenPracticeStuff")
do ignore <| (config.Port = 8888)

let server = new NetServer(config)
do server.Start()

server.