const colors = require('tailwindcss/colors')

/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        "**/*.razor",
        "**/*.razor.cs",
        "**/*.cshtml",
        "**/*.html"
    ],
    theme: {
        extend: {},
        colors: {
            gray: colors.gray,
            blue: colors.blue,
            red: colors.red,
            green: colors.green,
            white: colors.white,
        },
    },
    plugins: [],
}

