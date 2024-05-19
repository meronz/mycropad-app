export function read(key) {
    return localStorage.getItem(key);
}

export function write(key, payload) {
    localStorage.setItem(key, payload);
}