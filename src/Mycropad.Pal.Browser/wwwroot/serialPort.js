export function open(usbVid, usbPid)
{
    const filters = [
        { usbVendorId: usbVid, usbProductId: usbPid },
    ];

    navigator.serial.requestPort({ filters }).then(port => {
        console.log(port.getInfo());
        return port;
    });
}

export function discardInBuffer()
{

}

export function discardOutBuffer()
{

}

export function read(buffer, offset, count)
{

}

export function close()
{

}

export function write(buffer, offset, count)
{

}
