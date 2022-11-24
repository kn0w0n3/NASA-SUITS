//---------------------------------------//
//               Includes                //
//---------------------------------------//
var bodyParser = require("body-parser"),
    methodOverride = require("method-override"),
    mongoose = require("mongoose"),
    express = require("express"),
    app = express();
    
//var teleId = "5ab5452165eee209862ee5e0";
//var switId = "5ab5452165eee209862ee5e1";
    
//var ObjectID = require('mongodb').ObjectID;

//---------------------------------------//
//                APP CONFIG             //
//---------------------------------------//
mongoose.connect("mongodb://localhost/telemetry_stream");
app.set("view engine", "ejs");
app.use(express.static("public"));
app.use(bodyParser.urlencoded({extended:true}));
app.use(methodOverride("_method"));


//----------------------------------------//
//      Schema setup and seeding          //
//----------------------------------------//
var teleSchema = new mongoose.Schema({
    i_suit_p: String,
    t_life_battery: String,
    t_life_oxygen: String,
    t_life_water: String,
    p_sub: String,
    t_sub: String,
    v_fan: String,
    t_eva: String,
    p_o2: String,
    rate_o2: String,
    cap_battery: String,
    p_h2o_g: String,
    p_h2o_l: String,
    p_sop: String,
    rate_sop: String
});

var switchSchema = new mongoose.Schema({
    batterAmpHigh: String,
    batteryVdcLow: String,
    suitPressLow: String,
    sop_on : String,
    sspe : String,
    suitPressHigh: String,
    o2HighUse: String,
    sopPressLow: String,
    fan_error : String,
    vent_error : String,
    co2High: String,
    vehicle_power : String,
    h2o_off : String,
    o2_off : String
});

var Tele = mongoose.model("Tele", teleSchema);
var Swit = mongoose.model("Swit", switchSchema);


// //add new telemetry data to the DB (seed)
//         Tele.create({
//             i_suit_p: "4",
//             t_life_battery: "10:00:00",
//             t_life_oxygen: "10:00:00",
//             t_life_water: "10:00:00",
//             p_sub:  "4",
//             t_sub: "85",
//             v_fan: "40000",
//             t_eva: "00:00:00",
//             p_o2: "950",
//             rate_o2: "1.0",
//             cap_battery: "30",
//             p_h2o_g: "16",
//             p_h2o_l: "16",
//             p_sop: "950",
//             rate_sop: "1.0"
//         }, function(err, tele){
//             if(err){
//                 console.log(err);
//             } else {
//                 console.log(tele);
//             }
//         });
// //add new switch data to the DB (seed)
//         Swit.create({
//          batterAmpHigh: "false",
//          batteryVdcLow: "false",
//          suitPressLow: "false",
//          sop_on : "false",
//          sspe : "false",
//          suitPressHigh : "false",
//          o2HighUse : "false",
//          sopPressLow : "false",
//          fan_error : "false",
//          vent_error : "false",
//          vehicle_power : "false",
//          co2High : "false",
//          h2o_off : "false",
//          o2_off : "false"
//         }, function(err, swit){
//             if(err){
//                 console.log(err);
//             } else {
//                 console.log(swit);
//             }
//         });

//retrieve all Tele data (multiple objects) from the DB
        // Tele.find({}, function(err, teles){
        //     if(err){
        //         console.log("Error: ");
        //         console.log(err);
        //     } else {
        //         console.log("All the tele data....");
        //         console.log(teles);
        //     }
        // });
        
//-------------------------------------------//
//                  ROUTES                   //
//-------------------------------------------//
//INDEX ROUTE
app.get("/", function(req, res){
    Tele.find({}, function(err, teles){
        if(err){
            console.log(err);
        } else {
           Swit.find({}, function(err, swits){
               if(err){
                    console.log(err);
               } else {
                    var obj = {teles, swits};
                    res.render("index", {obj:obj});
               }
           });
        }
    });
});

//NUMERICAL ROUTE
//{ "_id" : ObjectId("5ab5452165eee209862ee5e0"), "p_sub" : "3", "t_sub" : "75", "v_fan" : "20000", "t_eva" : "00:00:00", "p_o2" : "850", "rate_o2" : "0.75", "cap_battery" : "15", "p_h2o_g" : "15", "p_h2o_l" : "15", "p_sop" : "850", "rate_sop" : "0.75" }
app.get("/numerical", function (req, res){
        //       var obj = {
        //         //_id   : {ObjectId(5ab5452165eee209862ee5e0)},
        //         p_sub : "3",       
        //         t_sub : "75",      
        //         v_fan : "20000",      
        //         t_eva : "00:01:02",
        //         p_o2  : "850",
        //         rate_o2 : "0.75",    
        //         cap_battery : "15",
        //         p_h2o_g : "15",    
        //         p_h2o_l : "15",     
        //         p_sop   : "850",    
        //         rate_sop: "0.75"
        //   };
    Tele.findOne({'_id':"5ab5452165eee209862ee5e0"}).exec(function(err, foundTele){     //problems getting findById to work
                if(err){
                    console.log(err);
                } else {
                    var jsonStr = JSON.stringify(foundTele);
                    res.send(jsonStr);
                    //console.log(teles);
                    //console.log(jsonStr);
                }
     });
});

//SWITCH ROUTE
//{ "_id" : ObjectId("5ab5452165eee209862ee5e1"), "sop_on" : "false", "sspe" : "false", "fan_error" : "false", "vent_error" : "false", "vehicle_power" : "false", "h2o_off" : "false", "o2_off" : "false" }
app.get("/switch", function(req, res){
    //       var switch = {
    //         //_id   : {ObjectId(5ab5452165eee209862ee5e1)},
    //          sop_on : "false",
    //          sspe : "false",
    //          fan_error : "false",
    //          vent_error : "false",
    //          vehicle_power : "false",
    //          h2o_off : "false",
    //          o2_off : "false"
    //   };
    Swit.findOne({'_id':"5ab5452165eee209862ee5e1"}).exec(function(err, foundSwit){     //problems getting findById to work
                if(err){
                    console.log(err);
                } else {
                    var jsonStr = JSON.stringify(foundSwit);
                    res.send(jsonStr);
                    //console.log(teles);
                    //console.log(jsonStr);
                }
     });
})


//DEFAULT GATEWAY
app.get("*", function(req, res){
    res.redirect("/");
});


//------------------------------------//
//           SERVER FUNCTIONS         //
//------------------------------------//

//server start
app.listen(process.env.PORT, process.env.IP, function(){
    console.log("server is running");
    teleUpdate();
});

//server update function (continuous)
function teleUpdate(){

        for(var i=0;i<5;i++){
            setTimeout(fanFail.bind(null,i), 8*i*1000*60);
        }
};

var fanFail = function(i){
        var array = [18000,16000,14000,12000,8000,3500,0000];
            Tele.findByIdAndUpdate("5ab5452165eee209862ee5e0", { $set: { "v_fan": array[i]} }, function(){
                
            });
            console.log("data changed");
}