
import React, { useState, useEffect } from "react";
import axios from "axios";
import "./App.css";
import AdvisorList from "./components/AdvisorList";
import AdvisorForm from "./components/AdvisorForm";

const App = () => {
  const [advisors, setAdvisors] = useState([]);
  const [form, setForm] = useState({
    fullName: "",
    sin: "",
    address: "",
    phoneNumber: "",
  });

  useEffect(() => {
    fetchAdvisors();
  }, []);

  const fetchAdvisors = async () => {
    try {
      const response = await axios("https://localhost:7064/api/Advisors/ListAdvisors");

      setAdvisors(response.data);
    } catch (error) {
      console.error("Error fetching advisors:", error);
    }
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setForm({ ...form, [name]: value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await axios.post(
        "https://localhost:7064/api/Advisors/CreateAdvisor",
        form
      );
      fetchAdvisors();
      setForm({ fullName: "", sin: "", address: "", phoneNumber: "" });
    } catch (error) {
      console.error("Error creating advisor:", error);
    }
  };

  const handleDelete = async (id) => {
    try {
      await axios.delete(
        `https://localhost:7064/api/Advisors/DeleteAdvisor/${id}`
      );
      fetchAdvisors();
    } catch (error) {
      console.error("Error deleting advisor:", error);
    }
  };

  return (
    <div className="App">      
      <AdvisorForm handleSubmit={handleSubmit} handleInputChange={handleInputChange} form={form}/>
      <AdvisorList advisors={advisors} handleDelete={handleDelete}/>
    </div>
  );
};

export default App;
