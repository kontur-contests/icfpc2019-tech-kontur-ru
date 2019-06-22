const path = require('path');
const fs = require('fs');

const filesImports = [
    'window.files = { cond: {}, desc: {} };'
];
const tasksNumbers = [];
const solutionsNumbers = [];

const dirUrl = '../problems/puzzles';
const directoryPath = path.join(__dirname, dirUrl);


const files = fs.readdirSync(directoryPath);
files.forEach(function (file) {
    const ext = file.split('.')[1];
    if (ext !== 'desc' && ext !== 'cond') {
        return;
    }

    const num = file.substr(5, 3);

    if (ext === 'cond') {
        tasksNumbers.push(num);
    } else {
        solutionsNumbers.push(num);
    }
});

for (let i = 1; i <= Math.max(tasksNumbers.length, solutionsNumbers.length); i++) {
    const number = i.toString().padStart(3, '0');
    const problemPath = `${dirUrl}/puzzle-${number}.cond`;
    const solutionPath = `${dirUrl}/task-${number}.desc`;
    filesImports.push(`window.files.cond['${number}'] = require('${problemPath}').default;`);
    filesImports.push(`window.files.desc['${number}'] = require('${solutionPath}').default;`);

    if (!fs.existsSync(problemPath)) {
        fs.writeFileSync(problemPath, '');
    }

    if (!fs.existsSync(solutionPath)) {
        fs.writeFileSync(solutionPath, '');
    }
}



const filePath = path.join(__dirname, 'files-puzzles.js');

fs.writeFileSync(filePath, filesImports.join('\n'));