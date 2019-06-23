import sys
from subprocess import Popen
from itertools import islice

if __name__ == '__main__':
    algo_prefix, start_task, end_task, parallelism = sys.argv[1], int(sys.argv[2]), int(sys.argv[3]), int(sys.argv[4])

    buildprocess = Popen("dotnet build -c Release console-runner/console-runner.csproj", shell=True)
    buildprocess.wait()
    if buildprocess.returncode != 0:
        exit(buildprocess.returncode)

    cmd = "dotnet run -c Release --project=console-runner/console-runner.csproj -- solve -s %s -p %d"
    processes = (Popen(cmd % (algo_prefix, task_num), shell=True) for task_num in range(start_task, end_task + 1))
    running_processes = list(islice(processes, parallelism))  # start new processes
    while running_processes:
        for i, process in enumerate(running_processes):
            if process.poll() is not None:  # the process has finished
                running_processes[i] = next(processes, None)  # start new process
                if running_processes[i] is None:  # no new processes
                    del running_processes[i]
                    break
