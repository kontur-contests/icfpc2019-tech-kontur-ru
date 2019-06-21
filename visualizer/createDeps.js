const path = require('path');
const fs = require('fs');

const filesImports = [
    'window.files = { desc: {}, sol: {} };'
];
const dirUrl = '../problems/all';
const directoryPath = path.join(__dirname, dirUrl);

const files = fs.readdirSync(directoryPath);
files.forEach(function (file) {
    const ext = file.split('.')[1];
    if (ext !== 'desc' && ext !== 'sol') {
        return;
    }

    const num = file.substr(5, 3);

    filesImports.push(`window.files['${ext}']['${num}'] = require('${dirUrl}/${file}').default;`);
});


const filePath = path.join(__dirname, 'files.js');

fs.writeFileSync(filePath, filesImports.join('\n'));