const path = require('path');

module.exports = {
    entry: './files.js',
    output: {
        filename: 'bundle.js',
    },
    module: {
        rules: [
            {
                test: /\.(desc|sol)$/i,
                use: 'raw-loader',
            },
        ],
    },
};