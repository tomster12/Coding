(()=>{"use strict";const o=new(require("socket.io").Server)(3e3);console.log("Listening on *:3000"),o.sockets.on("connection",(o=>{console.log(`Connected ${o.id}`),o.on("getLocation",((o,e)=>{e("Hello client")})),o.on("disconnect",(()=>{console.log(`Disconnected ${o.id}`)}))}))})();