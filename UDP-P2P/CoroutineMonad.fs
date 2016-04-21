module CoroutineMonad

type Coroutine<'s, 'a> = 's -> CoroutineStep<'s, 'a>
and CoroutineStep<'s, 'a> =
  | Done of 's*'a
  | Yield of 's*Coroutine<'s, 'a>

let return_ x = fun s -> Done(s, x)

let rec bind_ p k =
  fun s ->
    match p s with
    | Done(s', a) -> k a s'
    | Yield(s', p') -> Yield(s', bind_ p' k)

type CoroutineBuilder() =
  member this.Return x = return_ x
  member this.ReturnFrom c = c
  member this.Bind(p,k) = bind_ p k
  member this.Zero() = return_ ()
let co = CoroutineBuilder()

let getState = fun s -> Done(s,s)

let setState x = fun s -> Done(x,x)

let yield_ = fun s -> Yield(s, (fun s -> Done(s, ())))

let wait_ interval =
  let time = fun s -> Done(s, System.DateTime.Now)
  co{
    let! t0 = time
    let rec wait_ () =
      co{
        let! t = time
        let dt = (t0-t).TotalSeconds
        if dt < interval then
          do! yield_
          return! wait_ ()
      }
    do! wait_ ()
  }

let printX x = fun s -> printfn "%A" x
                        Done(s, ())

let readX = fun s -> Done(s, System.Console.ReadLine())

let rec costep c s =
  match c s with
  | Done(s', a) -> s', return_ a
  | Yield(s', c') -> s', c'