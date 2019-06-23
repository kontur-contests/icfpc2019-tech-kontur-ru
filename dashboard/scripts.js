const TEN_MINUTES = 10 * 60 * 1000;
const body = document.getElementsByTagName('body')[0];

let hiddenAlgs = getHiddenColumns();
let showTable = getState();
// let filterStr = '';
// let algsListOpen = false;
const formattedData = mapBaseData(dataFromServer);
const formattedBlockchainData = mapData(blockchainDataForScript);
const formattedDataProgress = mapData(progressDataForScript);
renderState();


function getHiddenColumns() {
    return JSON.parse(localStorage.getItem('hiddenColumn')) || [];
}

function getState() {
    return localStorage.getItem('showTable') || 'base';
}

function mapBaseData(rawData) {
    const algs = new Set();
    const tasks = new Set();
    const formattedData = rawData.reduce((acc, item) => {
        if (!item._id) {
            return acc;
        }
        const [id, alg, version, money] = item._id.split('_');
        const algName = `${alg} v${version}`;

        tasks.add(parseInt(id));
        algs.add(algName);

        if (!acc[id]) {
            acc[id] = {}
        }

        if (!acc[id][algName]) {
            acc[id][algName] = {}
        }

        acc[id][algName][money] = {
            ...item,
        };

        return acc;
    }, {});

    return {
        algs: [...algs].sort(),
        tasks: [...tasks].sort((a, b) => a - b),
        data: formattedData,
    }
}

function mapData(rawData) {
    const algs = new Set();
    const tasks = new Set();
    const formattedData = rawData.reduce((acc, item) => {
        const [id, alg, version] = item._id.split('_');
        const algName = `${alg} v${version}`;

        tasks.add(parseInt(id));
        algs.add(algName);

        if (!acc[id]) {
            acc[id] = {}
        }

        acc[id][algName] = {
            ...item,
        };

        return acc;
    }, {});

    return {
        algs: [...algs].sort(),
        tasks: [...tasks].sort((a, b) => a - b),
        data: formattedData,
    }
}

function renderState() {
    renderControls();
    let data;
    let needProgress = false;
    let bests;


    if (showTable === 'base') {
        data = formattedData;
        bests = calcBaseBests(data.data)
    } else {
        data = formattedBlockchainData;
        bests = calcBests(data.data);

    }


    renderTable(data.data, data.algs, data.tasks, bests);

}

function renderControls() {
    let wrapper;

    if (!document.getElementsByClassName('controls').length) {
        wrapper = document.createElement('div');
        wrapper.classList.add('controls');
        body.appendChild(wrapper);
    } else {
        wrapper = document.getElementsByClassName('controls')[0];
    }

    renderToggle(wrapper);

    // renderFilter();

    if (showTable === 'base') {
        renderShowAllButton();
        // renderOpenAlgsListButton();
        // renderAlgsList();
    }
}

function renderFilter() {
    const container = document.getElementsByClassName('controls')[0];
    const form = document.createElement('form');
    form.addEventListener('submit', handleFilterSubmit);
    const filter = document.createElement('input');
    filter.classList.add('filter');

    form.appendChild(filter);
    container.appendChild(form)
}

function renderShowAllButton() {
    if (!hiddenAlgs || !hiddenAlgs.length ||
        document.getElementsByClassName('show-all').length ||
        showTable !== 'base') {
        return;
    }

    const container = document.getElementsByClassName('controls')[0];
    const button = document.createElement('button');
    button.textContent = 'Показать все алгоритмы';
    button.classList.add('show-all');
    button.addEventListener('click', showAll);

    container.appendChild(button);

}

function renderOpenAlgsListButton() {
    if (document.getElementsByClassName('list-toggle').length || showTable !== 'base') {
        return;
    }

    const container = document.getElementsByClassName('controls')[0];
    const button = document.createElement('button');
    button.textContent = `${algsListOpen ? 'Закрыть' : 'Открыть' } список алгоритмов`;
    button.classList.add('list-toggle');
    button.addEventListener('click', toggleAlgsList);

    container.appendChild(button);
}

function renderToggle(container) {
    if (document.getElementsByClassName('toggle').length) {
        return;
    }

    const toggle = document.createElement('label');
    toggle.classList.add('toggle');

    const checkbox = document.createElement('input');
    checkbox.classList.add('toggle-input');
    checkbox.type = 'checkbox';
    checkbox.checked = showTable !== 'base';
    checkbox.addEventListener('change', stateUpdate);

    const rightText = document.createElement('span');
    rightText.classList.add('toggle-text');
    rightText.textContent = 'Показывать паззлы';

    toggle.appendChild(checkbox);
    toggle.appendChild(rightText);
    container.appendChild(toggle);

}

function renderAlgsList() {
    if (document.getElementsByClassName('algs-wrapper').length) {
        const deleting = document.getElementsByClassName('algs-wrapper')[0];
        deleting.parentNode.removeChild(deleting);
    }

    if (!algsListOpen) {
        return
    }

    const container = document.getElementsByClassName('controls')[0];
    const list = document.createElement('div');
    list.classList.add('algs-wrapper')


}

function stateUpdate(e) {
    const value = e.target.checked;
    showTable = value ? 'puzzles' : 'base';

    localStorage.setItem("showTable", showTable);

    deleteShowAllButton();
    deleteTable();
    renderState();
}

function calcBests(data) {
    const bests = {};
    for (const taskNum of Object.keys(data)) {
        const taskTries = data[taskNum];
        for (const algName of Object.keys(taskTries)) {
            const algRes = taskTries[algName];
            algRes.algName = algName;
            if (!bests[taskNum] || bests[taskNum].time > algRes.time) {
                bests[taskNum] = algRes;
            }
        }
    }

    return bests;
}

function calcBaseBests(data) {
    const baseBests = {};

    for (const num of Object.keys(data)) {
        baseBests[num] = {};
        const task = data[num];
        for (const algName of Object.keys(task)) {
            const algRes = task[algName];

            if (algRes[0] && (!baseBests[num].time || algRes[0].time < baseBests[num].time)) {
                baseBests[num] = {
                    time: algRes[0].time,
                    algName,
                }
            }
        }
    }


    for (const num of Object.keys(data)) {
        const task = data[num];
        for (const algName of Object.keys(task)) {
            const algRes = task[algName];
            for (const cost of Object.keys(algRes)) {

                const mapScore = Math.log2(100 * 100) * 1000;
                const prevScore = Math.ceil(mapScore * algRes[cost].time / baseBests[num].time);
                const nextScore = Math.ceil(mapScore);

                const nextScoreWithCost = nextScore - cost;

                algRes[cost].weightedRes = nextScoreWithCost - prevScore;
            }
        }
    }

    const bestWeighted = {};

    for (const num of Object.keys(data)) {
        bestWeighted[num] = {};
        const task = data[num];
        for (const algName of Object.keys(task)) {
            const algRes = task[algName];
            for (const cost of Object.keys(algRes)) {
                if (bestWeighted[num].weightedRes === undefined  || algRes[cost].weightedRes > bestWeighted[num].weightedRes) {
                    bestWeighted[num] = {
                        ...algRes[cost],
                        cost: cost,
                        algName: algName,
                    };
                }
            }
        }
    }


    return bestWeighted;
}

function renderTable(data, algs, tasks, bests) {
    const table = document.createElement('table');
    table.classList.add('table');

    const algsOrder = algs.filter(alg => !hiddenAlgs.includes(alg));

    const tableHeader = createTableHeader(algsOrder);

    const tableBody = createTableBody(data, algsOrder, tasks, bests);

    table.appendChild(tableHeader);
    table.appendChild(tableBody);
    body.appendChild(table);


    if (showTable === 'base') {
        addListeners();
    }
}

function createTableHeader(algs) {
    const tableHeader = document.createElement('thead');
    const tableHeaderRow = document.createElement('tr');

    const indexTh = document.createElement('th');
    tableHeaderRow.appendChild(indexTh);

    const bestTh = document.createElement('th');
    bestTh.innerText = 'best score';
    tableHeaderRow.appendChild(bestTh);



    algs.forEach(item => {
        const th = document.createElement('th');
        th.textContent = `${item}`;

        tableHeaderRow.appendChild(th);
    });

    tableHeader.appendChild(tableHeaderRow);

    return tableHeader;
}

function createTableBody(data, algs, tasks, bests) {
    const tableBody = document.createElement('tbody');

    tasks.sort((a, b) => showTable === 'base' ? a - b : b - a).forEach(task => {
        const tr = document.createElement('tr');
        const indexTd = document.createElement('td');
        indexTd.textContent = task;
        tr.appendChild(indexTd);


        const bestTd = document.createElement('td');
        bestTd.classList.add('min');
        bestTd.innerHTML = `<b>${bests[task].time}</b><br>${bests[task].algName}<br>Денег тратит: ${bests[task].cost}`;
        tr.appendChild(bestTd);

        tableBody.appendChild(tr);

        const rowData = data[task];

        algs.forEach(alg => {
            const data = rowData[alg];
            let td;
            if (showTable === 'base') {
                td = createBaseCell(data, alg, task, bests);
            } else {
                td = createCell(data, alg, task, bests);
            }
            tr.appendChild(td);
        });
    });

    return tableBody;
}

function createCell(data, algName, taskNum, bests) {
    const td = document.createElement('td');
    td.textContent = (data && data.time) || '';

    if (!data || !data.time) {
        td.classList.add('no');
    } else if (bests[taskNum].algName === algName) {
        td.classList.add('min');
    }

    const now = Date.now();
    if (data && now - data.timestamp * 1000 < TEN_MINUTES) {
        td.classList.add('recent');
        td.setAttribute('title', `Посчитан недавно`)
    }

    return td;
}

function createBaseCell(data, algName, taskNum, bests) {
    const td = document.createElement('td');

    if (!data) {
        td.textContent = '';
        td.classList.add('no');

        if (formattedDataProgress.data[taskNum] && formattedDataProgress.data[taskNum][algName]) {
            td.classList.add('in-progress');
            td.textContent = '⏳';
            td.setAttribute('title', `Алгоритм выполняется на ${formattedDataProgress.data[taskNum][algName].hostName}`)
        }

        return td;
    }

    td.innerHTML =  Object.keys(data).reduce((acc, money) => {
        const chunk =  `<span class="rowInCell"><b>${money}: </b> ${data[money].time}</span><br>`
        return acc + chunk;
    }, '');


    if (bests[taskNum].algName === algName) {
        td.classList.add('min');
    }
    //
    // const now = Date.now();
    // if ( now - data.timestamp * 1000 < TEN_MINUTES) {
    //     td.classList.add('recent');
    //     td.setAttribute('title', `Посчитан недавно`)
    // }

    return td;
}

function addListeners() {
    const ths = [...document.getElementsByTagName('th')];

    for (let i = 0; i < ths.length; i++) {
        ths[i].addEventListener('click', hideColumn)
    }

}

function deleteTable() {
    const table = document.getElementsByClassName('table')[0];

    if (table) {
        body.removeChild(table);
    }
}

function hideColumn(e) {
    const algId = e.target.innerText;

    if (!hiddenAlgs) {
        hiddenAlgs = [];
    }

    hiddenAlgs.push(algId);

    localStorage.setItem("hiddenColumn", JSON.stringify(hiddenAlgs));

    deleteTable();
    renderShowAllButton();
    renderState();
}

function showAll() {
    hiddenAlgs = [];

    localStorage.setItem("hiddenColumn", JSON.stringify(hiddenAlgs));

    deleteTable();
    renderState();

    deleteShowAllButton();

}

function handleFilterSubmit(e) {
    const filter = document.getElementsByClassName('filter')[0];
    filterStr = filter.value;

}

function toggleAlgsList() {
    algsListOpen = !algsListOpen;

    deleteToggleListButton();
    renderState();

}

function deleteShowAllButton() {
    const button = document.getElementsByClassName('show-all')[0];

    if (button) {
        button.parentNode.removeChild(button);
    }
}

function deleteToggleListButton() {
    const button = document.getElementsByClassName('list-toggle')[0];

    if (button) {
        button.parentNode.removeChild(button);
    }
}
