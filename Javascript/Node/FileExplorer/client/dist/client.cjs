(()=>{"use strict";var e={n:t=>{var s=t&&t.__esModule?()=>t.default:()=>t;return e.d(s,{a:s}),s},d:(t,s)=>{for(var r in s)e.o(s,r)&&!e.o(t,r)&&Object.defineProperty(t,r,{enumerable:!0,get:s[r]})},o:(e,t)=>Object.prototype.hasOwnProperty.call(e,t)};const t=require("blessed");var s=e.n(t);const r=require("socket.io-client");var n=e.n(r);class i{constructor(){this.isConstructed=!0,this.promptCanInput=!1,this.promptIsInputting=!1,this.promptToClear=!1,this.onDestroy=null,this.initElements(),this.el_pnlb.focus(),this.el_screen.render()}initElements(){this.el_screen=s().screen({title:"File Explorer",smartCSR:!0,useBCE:!0,ignoreDockContrast:!0}),this.el_menu=s().listbar({parent:this.el_screen,height:3,keys:!0,autoCommandKeys:!1,border:{type:"line"},style:{prefix:{fg:"green"},item:{fg:"white"},focus:{selected:{bg:"blue"},border:{fg:"blue"}}},commands:[{prefix:"q",text:"exit",callback:()=>{this.destroy()}},{prefix:"o",text:"options",callback:()=>{this.log("[Options]\n")}},{prefix:"h",text:"help",callback:()=>{this.log("[Help]\n")}}]}),this.el_statusicon=s().box({parent:this.el_menu,top:0,right:1,width:2,height:1,style:{bg:"red"}}),this.el_pnll=s().box({parent:this.el_screen,left:0,top:3,bottom:3,width:i.PNLL_WIDTH,border:{type:"line"},style:{border:{fg:"white"},focus:{border:{fg:"blue"}}}}),this.el_pnlr=s().box({parent:this.el_screen,right:0,top:3,bottom:3,width:`100%-${i.PNLL_WIDTH}`,scrollable:!0,alwaysScroll:!0,keys:!0,scrollbar:{style:{bg:"green"},track:{bg:"#ffffff"}},border:{type:"line"},style:{border:{fg:"white"},focus:{border:{fg:"blue"}}}}),this.el_pnlb=s().box({parent:this.el_screen,left:0,right:0,bottom:0,width:"100%",height:3,tags:!0,content:"{green-fg}i{/}:>",border:{type:"line"},style:{border:{fg:"white"},focus:{border:{fg:"blue"}}}}),this.el_prompt=s().textbox({parent:this.el_pnlb,left:4,right:2,keys:!0,tags:!0,style:{bg:"null",fg:"white",focus:{bg:"blue",fg:"white"}}}),this.el_prompt.setValue("Type 'help' for a list of commands..."),this.promptIsInputting=!1,this.promptToClear=!0,this.el_pnll.key("tab",((e,t)=>this.el_pnlr.focus())),this.el_pnlr.key("tab",((e,t)=>this.el_pnlb.focus())),this.el_pnlb.key("tab",((e,t)=>this.el_menu.focus())),this.el_menu.key("tab",((e,t)=>this.el_pnll.focus())),this.el_screen.key("q",((e,t)=>{this.el_menu.focus(),this.el_menu.selectTab(0)})),this.el_screen.key("o",((e,t)=>{this.el_menu.focus(),this.el_menu.selectTab(1)})),this.el_screen.key("h",((e,t)=>{this.el_menu.focus(),this.el_menu.selectTab(2)})),this.el_screen.key("i",((e,t)=>{this.promptStart()})),this.el_pnlr.on("focus",(()=>{this.el_pnlr.setScroll(this.el_pnlr.getLines().length-1),this.el_screen.render()})),this.el_prompt.key("tab",((e,t)=>{this.promptInterrupt()&&(this.el_menu.focus(),this.el_screen.render())})),this.el_pnlb.key("enter",((e,t)=>{this.promptStart()&&this.el_screen.render()})),this.el_pnlb.key("backspace",((e,t)=>{this.promptToClear=!0,this.promptStart()&&this.el_screen.render()})),this.el_prompt.key("enter",((e,t)=>{this.promptSubmit()&&this.el_screen.render()})),this.el_prompt.key("escape",((e,t)=>{this.promptInterrupt()&&this.el_screen.render()})),this.el_menu.on("blur",(()=>{this.el_menu.select(0),this.el_screen.render()}))}promptStart(){return!!this.promptCanInput&&!this.promptIsInputting&&(this.promptToClear&&(this.el_prompt.clearValue(),this.promptToClear=!1),this.el_prompt.readInput(),this.el_screen.render(),this.promptIsInputting=!0,!0)}promptSubmit(){if(!this.promptIsInputting)return!1;const e=this.el_prompt.getValue();return this.submitCommand(e),this.el_prompt.submit(),this.promptIsInputting=!1,this.el_prompt.setValue("Type 'help' for a list of commands..."),this.promptToClear=!0,!0}promptInterrupt(){if(!this.promptIsInputting)return!1;let e=this.el_prompt.getValue().trimEnd();return this.el_prompt.setValue(e),this.el_prompt.cancel(),this.promptIsInputting=!1,""==this.el_prompt.getValue()&&(this.el_prompt.setValue("Type 'help' for a list of commands..."),this.promptToClear=!0),!0}submitCommand(e){""!=(e=e.trimEnd())&&this.log("> "+e+"\n")}log(e){this.el_pnlr.pushLine(e),this.el_pnlr.setScroll(this.el_pnlr.getLines().length-1),this.el_screen.render()}destroy(){this.isConstructed&&(this.isConstructed=!1,this.el_screen.destroy(),null!=this.onDestroy&&this.onDestroy())}setStatusIcon(e){const t=e?"green":"red";this.el_statusicon.style.bg=t}}i.PNLL_WIDTH=55,new class{constructor(){this.initScreen(),this.initConnection()}initScreen(){this.screen=new i,this.screen.onDestroy=()=>this.onScreenDestroy()}initConnection(){this.socket=n()("http://localhost:3000"),this.screen.log("Connecting..."),this.socket.on("connect",(()=>{this.onConnected()})),this.socket.on("disconnect",(()=>{this.onDisconnected()}))}logWelcome(){this.screen.log("Welcome to File Explorer! Type 'help' into the prompt to get started.\n")}onScreenDestroy(){this.socket.disconnect()}onConnected(){this.screen.promptCanInput=!0,this.screen.setStatusIcon(!0),this.screen.log("Connected.\n"),this.logWelcome(),this.socket.emit("testMessage","Hello server",(e=>{this.screen.log("Received: "+e)}))}onDisconnected(){this.screen.isConstructed?(this.screen.promptCanInput=!1,this.screen.setStatusIcon(!1),this.screen.log("Disconnected."),this.screen.log("Reconnecting...\n")):console.log("File Explorer exited.")}}})();