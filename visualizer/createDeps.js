const path = require('path');
const fs = require('fs');

const filesImports = [
    'window.files = { desc: {}, sol: {} };'
];
const tasksNumbers = [];
const solutionsNumbers = [];

const dirUrl = '../problems/all';
const directoryPath = path.join(__dirname, dirUrl);


const files = fs.readdirSync(directoryPath);
files.forEach(function (file) {
    const ext = file.split('.')[1];
    if (ext !== 'desc' && ext !== 'sol') {
        return;
    }

    const num = file.substr(5, 3);

    if (ext === 'desc') {
        tasksNumbers.push(num);
    } else {
        solutionsNumbers.push(num);
    }
});

for (let i = 1; i <= Math.max(tasksNumbers.length, solutionsNumbers.length); i++) {
    const number = i.toString().padStart(3, '0');
    const fileName = `prob-${number}`;
    const problemPath = `${dirUrl}/${fileName}.desc`;
    const solutionPath = `${dirUrl}/${fileName}.sol`;
    filesImports.push(`window.files.desc['${number}'] = require('${problemPath}').default;`);
    filesImports.push(`window.files.sol['${number}'] = require('${solutionPath}').default;`);

    if (!fs.existsSync(problemPath)) {
        fs.writeFileSync(problemPath, '');
    }

    if (!fs.existsSync(solutionPath)) {
        fs.writeFileSync(solutionPath, '');
    }
}



const filePath = path.join(__dirname, 'files.js');

fs.writeFileSync(filePath, filesImports.join('\n'));