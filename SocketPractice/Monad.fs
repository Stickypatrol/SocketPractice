module Monad

type Cstate<'a, 's> = 's -> CstateStep<'a, 's>
and CstateStep<'a, 's> =
  | Done of 'a* 's
  | NotDone of Cstate<'a, 's> * 's

let ret a = fun s -> a, s

let rec bind (p:Cstate<'a, 's>, k:'a -> Cstate<'b, 's>) : Cstate<'b, 's> =
  fun s ->
    match p s with
    | Done (a, s') -> k a s'
    | NotDone (p', s') -> NotDone(bind (p', k), s')