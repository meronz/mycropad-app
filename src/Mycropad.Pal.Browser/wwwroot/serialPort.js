let gSerialPort = null;
let gDotnetReference = null;

export async function open(usbVid, usbPid, dotnetReference)
{
    gDotnetReference = dotnetReference;

    const filters = [
        { usbVendorId: usbVid, usbProductId: usbPid },
    ];

    if(!gSerialPort) {
        gSerialPort = await navigator.serial.requestPort({filters});
        gSerialPort.ondisconnect = async () => {
            gSerialPort = null;
            await gDotnetReference.invokeMethodAsync('OnDisconnect');
        }
    }

    await gSerialPort.open({ baudRate: 115200 });
}

export function discardInBuffer()
{

}

export function discardOutBuffer()
{

}

export async function read() {
    const reader = gSerialPort.readable.getReader();
    const result = await reader.read();
    reader.releaseLock();
    return result.value;
}

export function close()
{
    gSerialPort.close();
}

export async function write(buffer) {
    const writer = gSerialPort.writable.getWriter();
    await writer.write(buffer);
    writer.releaseLock();
}
