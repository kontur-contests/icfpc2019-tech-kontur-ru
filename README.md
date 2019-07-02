# Team tech.kontur.ru @ ICFP Contest 2019

There were 8 members in team [tech.kontur.ru](https://tech.kontur.ru) at ICFP Contest 2019, and here's what we've done.

## Code breakdown

In this 72-hour contest, we had to solve tasks by creating an AI for robots which wrap (i.e., paint) 2-dimensional maps. After 24-hours, we also could solve tasks in a team-contributed "blockchain". 

**Shared domain code.** [lib/Models](./lib/Models) contains the code used by task solvers to represent tasks and AI state as well as some utility code. [tests](./tests/) helped to chase bugs in this code.

**Task solvers.** [lib/Solvers](./lib/Solvers) includes a number of task solvers which were good enough to make it to the end of the contest:

* **[Greedy](./lib/Solvers/GreedySolver.cs), [Stupid](./lib/Solvers/StupidSolver.cs).** Fast and non-optimal solvers which were able to compute solutions for all provided maps in reasonable time. These solvers enabled us to submit solutions for all problems early in the Lightning Round
* **[Palka](./lib/Solvers/PalkaSolver.cs).** A promising solver which used map features (so called "boosters") to make the robot grow in particular direction (i.e., to grow a stick, a "palka" in Russian) and then wrap the map efficiently 
* **[RandomWalk](./lib/Solvers/RandomWalk/RandomWalkSolver.cs), [DeepWalk](./lib/Solvers/RandomWalk/DeepWalkSolver.cs), [ParallelDeepWalk](./lib/Solvers/RandomWalk/ParallelDeepWalkSolver.cs).** Single- and multi-robot ("parallel") solvers which construct solutions by computing a number of suboptimal random moves and selecting the best ones. They use B/F/C-boosters and estimate moves in such way that a robot sequentially wraps partially unwrapped closely connected parts of a map. These solvers enabled us to take the lead in the Lightning Round
* **[Plan](./lib/Solvers/RandomWalk/PlanSolver.cs) & [Paraplan](./lib/Solvers/RandomWalk/ParallelPlanSolver.cs).** Single- and multi-robot solvers which wrap the map according to an optimal plan suggested by a clusterization algorithm. See clusterization code in [MishaResearch](./MishaResearch/), results in [clusters.v1](./clusters.v1/) and [clusters.v2](./clusters.v2/)
* **[Postprocessor](./lib/Solvers/Postprocess/).** The ultimate solver which significantly improves the quality of other solvers' solutions via the rearrangement of AI moves. We've completed this solver during the last hours of the Full Round

**Computing infrastructure.** [console-runner](./console-runner/) was used to run the following tasks on our laptops and workstations, on multi-core virtual machines, and on a CI server:

* compute a solution for a particular task (or tasks) using a particular solver (or solvers)
* compute solutions for all task-solver combinations (if not computed before)
* compute a solution for the most recent task in the "blockchain"
* verify solution correctness via the "online checker" provided by organizers
* pack and submit best solutions to organizers
* verify that submitted solutions are accepted by organizers, show quality improvements

[pipeline](./pipeline/) code was used to store/retrieve solutions from MongoDB and use an "online checker" provided by organizers via Selenium WebDriver.

**Visualisation.** [visualizer](./visualizer/) contains a modified version of the "online visualizer" provided by organizers. Its obfuscated ScalaJS code was reverse-engineered and altered to implement features required to efficiently debug our solutions: loading tasks and solutions by their ids (bypassing the file selection dialog), step-by-step execution, time travel, etc.

A web [dashboard](./dashboard/) was used to monitor the progress of solution computation and compare the performance of different task solvers.

We've also generated [bitmap images](./problems/all/images/) for all maps to efficiently explore them bypassing the "online visualizer".

| Task 59 | Task 101 | Task 134 | Task 190 |
| - | - | - | - |
| ![](./raw/master/problems/all/images/59.png) | ![](./raw/master/problems/all/images/101.png) | ![](./raw/master/problems/all/images/134.png) | ![](./raw/master/problems/all/images/190.png) |

**Submissions.** [submission-grades](./submission-grades/) contains the results of every submission we've made during the contest.

### Building and running the code

Dependencies:
* .NET Core 2.2
* NodeJS 10.0 — visualizer
* Python 3 — clusterization

Run `dotnet run --project console-runner/console-runner.csproj -- -h` to see all available commands.

Run `dotnet run --project console-runner/console-runner.csproj -- solve [-p x..y] [-s solver-name-prefix]` to run any solver registered in `lib/Solvers/RunnableSolvers.cs` on problems with ids from `x` to `y`.

An even better way to run the solvers is to use the `StupidOne1` test in `tests/Solvers/SolversTests.cs`.

## Members

* [Alexey Kungurtsev](https://github.com/KungA) — algorithms, task solvers
* [Andrew Kostousov](https://github.com/AndrewKostousov), [Ivan Dashkevich](https://github.com/spaceorc) — algorithms, task solvers, shared domain code
* [Pavel Egorov](https://github.com/xoposhiy) — algorithms, task solvers, visualisation
* [Michael Khrushchev](https://github.com/MichaelEk) — data science for task solvers
* [Alexey Kirpichnikov](https://github.com/beevee) — computing infrastructure, submission quality assurance
* [Veronika Samokhina](https://github.com/aminopyridin) — visualisation, photography
* [Igor Lukanin](https://github.com/igorlukanin) — computing infrastructure, visualisation, catering, gonzo journalism

## Feedback about the contest

This contest should be marked for paying tribute to a number of previous contests. We were glad to notice the legacy of many renowned contests in this contest.

Also, kudos to organizers for making all contest infrastructure work super-smoothly as well as for frequent and clear communications with participants. The best organizers of a few recent years!

## Self-nomination for judges’ prize

We hope somebody likes the idea behind our "postprocessor" solver (see above).

It's also worth mentioning that we were making some field notes during the contest and broadcasting them to ~1k followers of our Telegram channel. It's in Russian but has some nice pics and animated images. Here's the start of the broadcast: https://t.me/KonturTech/416

More on our more-than-10-year history of participation in the ICFP Contest here: https://tech.kontur.ru/contests/icfpc