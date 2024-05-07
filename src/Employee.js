import React, { Component } from "react";
import { variables } from "./Variables";

class Employee extends Component {
  constructor(props) {
    super(props);

    this.state = {
      employees: [],
      modalTitle:"",
      employeeId:0,
      employeeName:"",
      error: null // Add an error state to handle fetch errors
    };
  }

  refreshList() {
    fetch('http://localhost:5295/Employee/GetAll')
      .then(response => {
        if (!response.ok) {
          throw new Error('Failed to fetch'); // Throw an error if response is not OK
        }
        return response.json();
      })
      .then(data => {
        console.log(data);
        this.setState({  employees: data.data, error: null }); // Clear error if fetch is successful
      })
      .catch(error => {
        this.setState({ error: error.message }); // Set error state if fetch fails
      });
  }

  componentDidMount() {
    this.refreshList();
  }

  changeEmployeeName =(e)=>{
    this.setState({EmployeeName:e.target.value});
  }

addClick(){
  this.setState({
    modalTitle:"Add Employee",
    EmployeeId:0,
    EmployeeName:''
  });
}

editClick(emp){
  this.setState({
    modalTitle:"Update Employee",
    EmployeeId:emp.employeeId,
    EmployeeName:emp.employeeName
  });
}

createClick(){
  fetch(variables.API_URL +'Employee/Save',{
    method:'POST',
    headers:{
      'Accept':'application/json',
      'Content-Type':'application/json'
    },
    body:JSON.stringify({
      employeeId:this.state.EmployeeId,
      employeeName:this.state.EmployeeName,
      department:'',
      dateOfJoining:new Date(),
      profileName:''
    })
  })
  .then(res=>res.json())
  .then((result)=>{
    alert(result.status);
    this.refreshList();
  },(error)=>{
    console.log("Failed");
  })
}

deleteClick(id){
if(window.confirm('Are you sure ')){
  fetch(variables.API_URL +'Employee/Delete?id='+id,{
    method:'DELETE',
    headers:{  
    'Accept':'application/json',
    'Content-Type':'application/json'
  }
  })
  .then((result)=>{
    alert(result.status);
    this.refreshList();
  })
}
}

  render() {
    const { employees, modalTitle,EmployeeId,EmployeeName, error } = this.state;

    if (error) {
      return <div>Error: {error}</div>; // Render error message if there is an error
    }

    return (
      <div>
        <table className="table table-striped">
          <thead>
            <tr>
              <th>EmployeeId</th>
              <th>EmployeeName</th>
              <th>Options</th>
            </tr>
          </thead>
          <tbody>
            {employees.map(emp => (
              <tr key={emp.employeeId}>
                <td>{emp.employeeId}</td>
                <td>{emp.employeeName}</td>
                <td>
                  <button type="button" className="btn btn-primary mr-1" data-bs-toggle="modal" data-bs-target="#exampleModal" onClick={()=>this.editClick(emp)}>
                    Edit
                  </button>
                  <button type="button" className="btn btn-danger mr-1" onClick={()=>this.deleteClick(emp.employeeId)}>
                    Delete
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>

    <button type="button" className="btn btn-primary m-2 float-end" data-bs-toggle="modal" data-bs-target="#exampleModal" onClick={()=>this.addClick()}>
      Add Employee
    </button>

    <div className="modal fade" id="exampleModal" tabIndex="-1" role="dialog" aria-hidden="true">
      <div className="modal-dialog modal-dialog-centered" role="document">
        <div className="modal-content">
          <div className="modal-header">
            <h5 className="modal-title" id="exampleModalLongTitle">{modalTitle}</h5>
            <button type="button" className="close" data-bs-dismiss="modal" aria-label="Close">
              <span aria-hidden="true">&times;</span>
            </button>
          </div>
          <div className="modal-body">
            <div className="input-group mb-3">
              <span className="input-group-text">Employee Name</span>
              <input type="text" className="form-control" value={EmployeeName} onChange={this.changeEmployeeName}>

              </input>
            </div>
            
          </div>
          <div className="modal-footer">
            <button type="button" className="btn btn-secondary" data-bs-dismiss="modal">Close</button>
            <button type="button" className="btn btn-primary" onClick={()=>this.createClick()}>Save changes</button>
          </div>
        </div>
      </div>
    </div>
      </div>
    );
  }
}

export default Employee;
