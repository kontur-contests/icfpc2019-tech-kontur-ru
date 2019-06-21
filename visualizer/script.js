const actionsWrapper = document.getElementById('main_section');


const controlCenter = document.createElement('div');
controlCenter.classList.add('control-center');

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



controlCenter.appendChild(prevButton);
controlCenter.appendChild(playButton);
controlCenter.appendChild(nextButton);
controlCenter.appendChild(resetButton);

actionsWrapper.appendChild(controlCenter);

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

function prevTick(e) {
    e.preventDefault();

    W().Pf.h(e);

    setTimeout(() => {
        for (let i = 0; i < currentTick; i++) {
            nextTick();
        }
    }, 50);


    currentTick--;
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