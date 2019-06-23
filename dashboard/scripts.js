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
    const mapSizes = getMapSizes();
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

                const mapScore = Math.log2(mapSizes[num]) * 1000;
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
        if (showTable === 'base') {
            bestTd.innerHTML = `<b>${bests[task].time}</b><br>${bests[task].algName}<br>Денег тратит: ${bests[task].cost}`;
        } else {
            bestTd.innerHTML = `<b>${bests[task].time}</b><br>${bests[task].algName}`;
        }
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

function getMapSizes() {
    return {
        "1": 24,
        "2": 1806,
        "3": 999,
        "4": 2079,
        "5": 1014,
        "6": 552,
        "7": 841,
        "8": 1023,
        "9": 572,
        "10": 1760,
        "11": 841,
        "12": 841,
        "13": 841,
        "14": 841,
        "15": 841,
        "16": 841,
        "17": 841,
        "18": 841,
        "19": 841,
        "20": 841,
        "21": 2401,
        "22": 2401,
        "23": 2401,
        "24": 2401,
        "25": 2156,
        "26": 2401,
        "27": 2401,
        "28": 2401,
        "29": 2401,
        "30": 2205,
        "31": 2401,
        "32": 2401,
        "33": 2401,
        "34": 2401,
        "35": 2401,
        "36": 2401,
        "37": 2401,
        "38": 2401,
        "39": 2401,
        "40": 2401,
        "41": 2401,
        "42": 2401,
        "43": 2401,
        "44": 2401,
        "45": 2401,
        "46": 2401,
        "47": 2401,
        "48": 2401,
        "49": 2401,
        "50": 2401,
        "51": 9900,
        "52": 9900,
        "53": 9900,
        "54": 8514,
        "55": 9900,
        "56": 9900,
        "57": 9900,
        "58": 9800,
        "59": 9900,
        "60": 9900,
        "61": 9800,
        "62": 9900,
        "63": 9900,
        "64": 9900,
        "65": 9900,
        "66": 9900,
        "67": 9900,
        "68": 9800,
        "69": 9800,
        "70": 9900,
        "71": 9408,
        "72": 9009,
        "73": 9702,
        "74": 9801,
        "75": 8613,
        "76": 9801,
        "77": 9801,
        "78": 9506,
        "79": 9604,
        "80": 9603,
        "81": 8536,
        "82": 9801,
        "83": 9801,
        "84": 9603,
        "85": 9702,
        "86": 9702,
        "87": 9801,
        "88": 9801,
        "89": 9801,
        "90": 9801,
        "91": 9801,
        "92": 9801,
        "93": 9603,
        "94": 9801,
        "95": 9702,
        "96": 8613,
        "97": 9801,
        "98": 9801,
        "99": 9702,
        "100": 9702,
        "101": 39800,
        "102": 39800,
        "103": 39601,
        "104": 37810,
        "105": 39800,
        "106": 39800,
        "107": 28457,
        "108": 39800,
        "109": 39800,
        "110": 39800,
        "111": 39800,
        "112": 39800,
        "113": 32636,
        "114": 39800,
        "115": 39800,
        "116": 39800,
        "117": 33233,
        "118": 39800,
        "119": 39800,
        "120": 39800,
        "121": 39601,
        "122": 39601,
        "123": 39402,
        "124": 39601,
        "125": 39004,
        "126": 39402,
        "127": 38805,
        "128": 39402,
        "129": 39601,
        "130": 39204,
        "131": 39204,
        "132": 39402,
        "133": 36477,
        "134": 38606,
        "135": 39601,
        "136": 39601,
        "137": 39006,
        "138": 39601,
        "139": 38606,
        "140": 39601,
        "141": 39204,
        "142": 37050,
        "143": 39601,
        "144": 38416,
        "145": 39402,
        "146": 36445,
        "147": 39402,
        "148": 39601,
        "149": 39601,
        "150": 39601,
        "151": 9900,
        "152": 9900,
        "153": 9900,
        "154": 9306,
        "155": 9009,
        "156": 9900,
        "157": 9800,
        "158": 9900,
        "159": 9800,
        "160": 9900,
        "161": 9801,
        "162": 9801,
        "163": 8648,
        "164": 9702,
        "165": 9603,
        "166": 9702,
        "167": 9801,
        "168": 9801,
        "169": 9801,
        "170": 9801,
        "171": 9801,
        "172": 9801,
        "173": 9801,
        "174": 9801,
        "175": 9801,
        "176": 9801,
        "177": 9801,
        "178": 9801,
        "179": 9801,
        "180": 9801,
        "181": 39800,
        "182": 39402,
        "183": 39800,
        "184": 39800,
        "185": 39800,
        "186": 39800,
        "187": 39600,
        "188": 39800,
        "189": 39402,
        "190": 39800,
        "191": 39601,
        "192": 39601,
        "193": 39601,
        "194": 38805,
        "195": 39601,
        "196": 39203,
        "197": 38805,
        "198": 38612,
        "199": 39601,
        "200": 39601,
        "201": 39601,
        "202": 39601,
        "203": 37014,
        "204": 39601,
        "205": 39601,
        "206": 39402,
        "207": 39601,
        "208": 39601,
        "209": 39203,
        "210": 39402,
        "211": 159600,
        "212": 159600,
        "213": 159600,
        "214": 159600,
        "215": 159600,
        "216": 158802,
        "217": 156408,
        "218": 159201,
        "219": 158802,
        "220": 158006,
        "221": 9702,
        "222": 9108,
        "223": 9900,
        "224": 9900,
        "225": 9900,
        "226": 9900,
        "227": 9207,
        "228": 9900,
        "229": 9900,
        "230": 9900,
        "231": 9801,
        "232": 9801,
        "233": 9801,
        "234": 9801,
        "235": 9405,
        "236": 9801,
        "237": 9801,
        "238": 9801,
        "239": 9801,
        "240": 9801,
        "241": 39800,
        "242": 39800,
        "243": 39800,
        "244": 39600,
        "245": 39800,
        "246": 39601,
        "247": 39800,
        "248": 39800,
        "249": 39800,
        "250": 39800,
        "251": 39402,
        "252": 39402,
        "253": 39004,
        "254": 39402,
        "255": 39601,
        "256": 39402,
        "257": 39402,
        "258": 35024,
        "259": 39601,
        "260": 39601,
        "261": 39402,
        "262": 39601,
        "263": 39601,
        "264": 39402,
        "265": 39402,
        "266": 37026,
        "267": 39006,
        "268": 39203,
        "269": 39601,
        "270": 39006,
        "271": 159600,
        "272": 159600,
        "273": 158802,
        "274": 159200,
        "275": 127281,
        "276": 159600,
        "277": 159600,
        "278": 159600,
        "279": 159600,
        "280": 89775,
        "281": 159600,
        "282": 159600,
        "283": 159600,
        "284": 159600,
        "285": 159600,
        "286": 159201,
        "287": 158404,
        "288": 109495,
        "289": 159201,
        "290": 155624,
        "291": 158802,
        "292": 159201,
        "293": 159201,
        "294": 158802,
        "295": 154822,
        "296": 79704,
        "297": 145125,
        "298": 139698,
        "299": 158006,
        "300": 155610,
        "533": 55225,
    }
}