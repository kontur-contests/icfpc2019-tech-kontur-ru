
const body = document.getElementsByTagName('body')[0];

let hiddenAlgs = getHiddenColumns();
let algs = new Set();
const formattedData = mapData(dataFromServer);
const formattedDataProgress = mapData(progressDataForScript);
const tenMinutes = 10 * 60 * 1000;
let bests = {};
calcBests();
renderShowAllButton();
createTable();


function getHiddenColumns() {
    return JSON.parse(localStorage.getItem('hiddenColumn')) || [];
}

function calcBests() {

    for (const taskNum of Object.keys(formattedData)) {
        const taskTries = formattedData[taskNum];
        for (const algName of Object.keys(taskTries)) {
            const algRes = taskTries[algName];
            algRes.algName = algName;
            if (!bests[taskNum] || bests[taskNum].time > algRes.time) {
                bests[taskNum] = algRes;
            }
        }
    }
}

function renderShowAllButton() {
    if (!hiddenAlgs || !hiddenAlgs.length || document.getElementsByClassName('show-all').length) {
        return;
    }


    const button = document.createElement('button');
    button.textContent = 'Показать все алгоритмы';
    button.classList.add('show-all');
    button.addEventListener('click', showAll);

    body.appendChild(button);

}

function createTable() {
    const table = document.createElement('table');
    table.classList.add('table');


    const tableHeader = document.createElement('thead');
    const tableHeaderRow = document.createElement('tr');

    const indexTh = document.createElement('th');
    tableHeaderRow.appendChild(indexTh);

    const bestTh = document.createElement('th');
    bestTh.classList.add('min');
    bestTh.innerText = 'best score';
    tableHeaderRow.appendChild(bestTh);

    const algsOrder = [...algs].sort().filter(alg => !hiddenAlgs.includes(alg));

    algsOrder.forEach(item => {
        const th = document.createElement('th');
        th.textContent = `${item}`;

        tableHeaderRow.appendChild(th);
    });

    const tableBody = document.createElement('tbody');

    Object.keys(formattedData).sort((a, b) => a - b).forEach((num) => {
        const tr = document.createElement('tr');
        const index = document.createElement('td');
        index.textContent = num;

        const bestTd = document.createElement('td');
        bestTd.classList.add('min');

        tr.appendChild(index);
        tr.appendChild(bestTd);

        const row = formattedData[num];
        algsOrder.forEach(item => {
            const td = document.createElement('td');
            td.textContent = (row[item] && row[item].time) || '';


            if (!row[item] || !row[item].time) {
                td.classList.add('no');
            } else if (bests[num].algName === item) {
                td.classList.add('min');
            }

            const now = Date.now();

            if (row[item] && now - row[item].timestamp * 1000 < tenMinutes) {
                td.classList.add('recent');
            }

            if (formattedDataProgress[num] && formattedDataProgress[num][item]) {
                if (!row[item] || !row[item].time) {
                    td.classList.add('in-progress');
                    td.textContent = '⏳';
                    td.setAttribute('title', `Алгоритм выполняется на ${formattedDataProgress[num][item].hostName}`)
                }
            }

            tr.appendChild(td);



            bestTd.innerHTML = `<b>${bests[num].time}</b><br>${bests[num].algName}`;

        });



        tableBody.appendChild(tr);
    });

    tableHeader.appendChild(tableHeaderRow);
    table.appendChild(tableHeader);
    table.appendChild(tableBody);
    body.appendChild(table);

    addListeners();
}

function mapData(rawData) {
    return rawData.reduce((acc, item) => {
        const [id, alg, version] = item._id.split('_');
        const algName = `${alg} v${version}`;


        algs.add(algName);

        if (!acc[id]) {
            acc[id] = {}
        }

        acc[id][algName] = {
            ...item,
        };

        return acc;
    }, {})
}



function addListeners() {
    const ths = [...document.getElementsByTagName('th')];

    for (let i = 0; i < ths.length; i++) {
        ths[i].addEventListener('click', hideColumn)
    }

}

function deleteTable() {
    const table = document.getElementsByClassName('table')[0];

    body.removeChild(table);
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
    createTable();
}

function showAll() {
    hiddenAlgs = [];

    localStorage.setItem("hiddenColumn", JSON.stringify(hiddenAlgs));

    deleteTable();
    createTable();


    const button = document.getElementsByClassName('show-all')[0];

    body.removeChild(button);
}
