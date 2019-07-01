# Team tech.kontur.ru @ ICFP Contest 2019

## How to build and run the solution

Dependencies:
* .NET Core 2.2 — to build and run the major part of source code (e.g., all solvers)
* NodeJS 10.0 — to build and run problem-solver-score visualizer
* Python 3 — to run map clustering algorithm

Run `dotnet run --project console-runner/console-runner.csproj -- -h` to see all available console commands.

Run `dotnet run --project console-runner/console-runner.csproj -- solve [-p x..y] [-s solver-name-prefix]` to run any solver registered in `lib/Solvers/RunnableSolvers.cs` on problems with ids from `x` to `y`.

An even better way to run the solvers is to use the `StupidOne1` test in `tests/Solvers/SolversTests.cs`.

## Description of the solution approach

We used many algorithms for solvers during the contest. The most notable ones are:
* simple greedy solvers (https://t.me/KonturTech/442, https://t.me/KonturTech/444) which let us submit solutions for all problems early in the Lightning Round
* an ensemble of "deep walk" solvers (https://t.me/KonturTech/446) which use B-boosters and try not to leave a partially unwrapped closely connected map parts — let us take the lead in the Lightning Round
* an ensemble of "parallel deep walk" solvers which use F/C-boosters to wrap map concurrently — these solvers give the bst results among all our "standalone" solvers
* the "postprocessor" solver which takes any solution by any other solver (e.g., a "parallel deep walk" solver), analyses and mutates it by rearranging moves — we've completed this solver during the last hours of the Full Round, and its results are mind-blowing, it basically makes better each and every solution by other "standalone" solvers

Other solvers rest in peace in the VCS history.

## Feedback about the contest

Kudos to organizers for making all infrastructure work super-smoothly and for frequent and clear communications with participants. The best organizers of the last years!

Also this contest should be marked for paying tribute to a number of previous contests. We were glad to notice that.

## Self-nomination for judges’ prize

We hope you'd like the idea behind our "postprocessor" solver (see above).

It's also worth mentioning that we were making some field notes during the contest and broadcasting them to ~1k followers of our Telegram channel. It's in Russian but has some nice pics and animated images. Here's the start of the broadcast: https://t.me/KonturTech/416

More on our more-than-10-year history of participation in the ICFP Contest here: https://tech.kontur.ru/contests/icfpc