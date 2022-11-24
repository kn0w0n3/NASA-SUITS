function DataPoint(name, passMin, passMax, failRate) {
    this.name = name;
    this.passMin = passMin;
    this.passMax = passMax;
    this.status = this._setStatus(failRate); 
    this.value = this._setValue(this.status);
}

//set a passing or failing status based on specified fail rate
DataPoint.prototype._setStatus = function(failRate) {
    const minRng = 0;
    const maxRng = 1e6;
    const randNum = (Math.random() * (maxRng - minRng)) + minRng;
    let threshold = (failRate/100) * maxRng;
    
    if(randNum > threshold) {
        return true;
    } else {
        return false;
    }
 };

 //set a passing or failing value 
DataPoint.prototype._setValue = function(status) {
    //if data point has a passing status
    if(status) {
        return (Math.random() * (this.passMax - this.passMin) + this.passMin).toFixed(2);
    } else {    //if data point has a failing status
        let max = 0;
        let min = 0;
        let value = 0;
        //if lower bound = 0, choose a value between upper bound  & (upperbound * .50) + upperbound                 
        //case where the lower bound can not be less than 0
        if(this.passMin === 0) {
            max = (this.passMax * .50) + this.passMax;
            min = this.passMax;
            value = (Math.random() * (max - min) + min);
            return value.toFixed(2);
        } else {    //for non-zero lower bounds
            //pick a value less than lower bound
            if(Math.random() * 100 < 50) {
                max = this.passMin;
                min = this.passMin - (this.passMin * .50);
                value = (Math.random() * (max-min) + min);
                return value.toFixed(2);
            } else {    //pick a value greater than upper bound
                max = this.passMax + (this.passMax * .50);
                min = this.passMax;
                value = (Math.random() * (max-min) + min);
                return value.toFixed(2);
            }
        }
    }
};

function makeTelemetry() {
    const failRate = 1; //1% fail rate
    const data = [];

    data.push(new DataPoint('internal_suit_pressure', 2, 4, failRate));
    data.push(new DataPoint('time_life_battery', 0, 10, failRate));
    data.push(new DataPoint('time_life_oxygen', 0, 10, failRate));
    data.push(new DataPoint('time_life_water', 0, 10, failRate));
    data.push(new DataPoint('sub_pressure', 2, 4, failRate));
    data.push(new DataPoint('sub_temperature', 30, 100, failRate)); //investigate actual value ranges
    data.push(new DataPoint('fan_tachometer', 10000, 40000, failRate));
    data.push(new DataPoint('extra_vehicular_time', 0, 9, failRate));
    data.push(new DataPoint('oxygen_pressure', 750, 950, failRate));
    data.push(new DataPoint('oxygen_rate', 0.5, 1, failRate));
    data.push(new DataPoint('battery_capacity', 0, 30, failRate));
    data.push(new DataPoint('h2o_gas_pressure', 14, 16, failRate));
    data.push(new DataPoint('h2o_liquid_pressure', 14, 16, failRate));
    data.push(new DataPoint('sop_pressure', 750, 950, failRate));
    data.push(new DataPoint('sop_rate', 0.5, 1, failRate));

    //an object literal that will hold numerical data point name and corresponding value
    const numericalTelemetry = {};

    for(const element of data) {
        const name = element.name;
        //will create key-pair value for each data point in array
        numericalTelemetry[name] = element.value;
    }

    return numericalTelemetry;
}

module.exports = makeTelemetry;

/*
    usage in telemetry.js

    const createTelemetry = require('./CreateTelemetry.js');

    //the function returns a telemetry object not a json object
    console.log(createTelemetry());

*/


