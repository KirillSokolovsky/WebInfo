var eventNames = [
    "focus",

    "reset",
    "submit",

    "compositionstart",
    "compositionupdate",
    "compositionend",

    "resize",
    "scroll",

    "keydown",
    "keypress",
    "keyup",

    "mouseenter",
    "mouseover",
    //"mousemove",
    "mousedown",
    "mouseup",
    "auxclick",
    "click",
    "dblclick",
    "contextmenu",
    "wheel",
    "mouseleave",
    "mouseout",
    "select",
    "pointerlockchange",
    "pointerlockerror",

    "dragstart",
    "drag",
    "dragend",
    "dragenter",
    "dragover",
    "dragleave",
    "drop",

    "broadcast",
    "CheckboxStateChange",
    "hashchange",
    "input",
    "RadioStateChange",
    "readystatechange",
    "ValueChange"
];

var log = console.log;

var capturedEvents = new Map();

function createCORSRequest(method, url) {
    var xhr = new XMLHttpRequest();
    if ("withCredentials" in xhr) {
      // XHR for Chrome/Firefox/Opera/Safari.
      xhr.open(method, url, true);
    } else if (typeof XDomainRequest != "undefined") {
      // XDomainRequest for IE.
      xhr = new XDomainRequest();
      xhr.open(method, url);
    } else {
      // CORS not supported.
      xhr = null;
    }
    return xhr;
  }

function sendData(data){

    var xhr = createCORSRequest('POST', "http://localhost:8899");
    if (!xhr) {
        alert('CORS not supported');
        return;
    }
    xhr.onload = function() {
        var text = xhr.responseText;
        log(text);
    };

    xhr.onerror = function() {
        //alert('Woops, there was an error making the request.');
      };
    
      xhr.send(JSON.stringify(data));      
}

function attributeInfo(acc, at){
    return {
        name: at.nodeName,
        value: at.nodeValue
    }
}

function elementInfo(acc, el){
    return { 
        name: el.localName,  
        attributes: el.attributes.reduce(attributeInfo)
    }
}

function handleClick(e){
    var eType = e.type;
    capturedEvents.set(eType, e);
    
    //console.log(eType);

    if(eType == "mouseover"){
        //var fromEl = e.fromElement as Element;

        if(e.fromElement != null)
            e.fromElement.classList.remove("my-enter");
        if(e.toElement != null)
            e.toElement.classList.add("my-enter");
    }

    var eventData = {
        EventName: eType,
        Path: Array.reduce(e.Path, elementInfo)
    };

    if(eType == "wheel"){
        log(e);
        //sendData({name: eType, ev: simpleKeys (e)});
    }
}

function start(){

    var style = document.createElement("style");
    style.type = "text/css";
    style.innerHTML = ".my-enter{ background-color: lightgoldenrodyellow; outline:1px solid blue; }";
    document.getElementsByTagName("head")[0].appendChild(style);

    var html = document.getElementsByTagName("html")[0];

    for(i = 0; i < eventNames.length; i++){
        html.addEventListener(eventNames[i], handleClick);
    }

    var b1 = document.getElementById("b1");
    b1.onclick = printEvs;
    console.log(b1);
}
function printEvs(){
    for(var [key, value] of capturedEvents.entries()){
        console.log(key);
        console.log(value);
    }
    capturedEvents.clear();
    //sendData("test");
}

start();

