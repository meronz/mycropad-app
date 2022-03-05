module.exports = {
    purge: {
        enabled: true,
        content: [
            './**/*.html',
            './**/*.razor'
        ],
    },
    darkMode: 'media', // or 'media' or 'class'
    theme: {
        extend: {},
    },
    variants: {
        extend: {
            mixBlendMode: ['hover'],
        },
    },
    plugins: [],
}
