const actionsWrapper = document.getElementById('main_section');

const controlCenter = document.createElement('div');
controlCenter.classList.add('control-center');

const firstRow = document.createElement('div');
firstRow.classList.add('row');

const prevButton = document.createElement('button');
prevButton.textContent = '<';
prevButton.addEventListener('click', prevTick);

const nextButton = document.createElement('button');
nextButton.textContent = '>';
nextButton.addEventListener('click', nextTick);

const playButton = document.createElement('button');
playButton.textContent = '▶︎||';
playButton.addEventListener('click', playPause);

const resetButton = document.createElement('button');
resetButton.classList.add('reset-button');
resetButton.textContent = '↺';
resetButton.addEventListener('click', reset);

const secondRow = document.createElement('div');
secondRow.classList.add('row');
secondRow.classList.add('second-row');

const prevFiveButton = document.createElement('button');
prevFiveButton.textContent = '<<';
prevFiveButton.addEventListener('click', prevFiveTick);

const nextFiveButton = document.createElement('button');
nextFiveButton.textContent = '>>';
nextFiveButton.addEventListener('click', nextFiveTick);


firstRow.appendChild(prevButton);
firstRow.appendChild(playButton);
firstRow.appendChild(nextButton);

secondRow.appendChild(prevFiveButton);
secondRow.appendChild(nextFiveButton);

controlCenter.appendChild(firstRow);
controlCenter.appendChild(secondRow);

actionsWrapper.appendChild(controlCenter);
actionsWrapper.appendChild(resetButton);



let intervalId = null;
let pause = true;
let currentTick = 0;


function playPause(e) {
    e.preventDefault();
    // const prepare = rm(Tl(), a.Pf);
    //
    // debugger;
    // if (isPause) {
    //     prepare();
    // }


    if (pause) {
        pause = false;
        intervalId = setInterval(() => {
            currentTick++;
            nextTick();
        }, 30)
    } else {
        stop();
    }
}

function nextTick(e) {
    if (e) {
        e.preventDefault();
        currentTick++;
    }

    const gameObj = W();
    gameObj.Hi = false;
    try {
        Wl();
    } catch (e) {
        stop();
    } finally {
        gameObj.Hi = true;
    }
}

function nextFiveTick(e) {
    if (e) {
        e.preventDefault();
    }

    for (let i = 0; i < 5; i++) {
        currentTick++;
        nextTick();
    }
}




function prevTick(e) {
    if (e) {
        e.preventDefault();
    }

    W().Pf.h(e);

    setTimeout(() => {
        for (let i = 0; i < currentTick; i++) {
            nextTick();
        }
    }, 50);


    currentTick--;
}

function prevFiveTick(e) {
    if (e) {
        e.preventDefault();
    }

    currentTick = currentTick - 4;
    currentTick = currentTick < 0 ? 0 : currentTick;
    prevTick();
}

function reset(e) {
    e.preventDefault();
    stop();
    W().Pf.h(e);
    W().Hi = true;
    currentTick = 0;
}

function stop() {
    pause = true;

    if (intervalId) {
        clearInterval(intervalId);
        intervalId = null;
    }
}