export default function AdvisorForm({ handleSubmit, handleInputChange, form }) {
  return (
    <div>
      <h1>Advisor Management</h1>

      <form onSubmit={handleSubmit}>
        <input
          name="fullName"
          placeholder="Full Name"
          value={form.fullName}
          onChange={handleInputChange}
          required
        />
        <input
          name="sin"
          placeholder="SIN (9 digits)"
          value={form.sin}
          onChange={handleInputChange}
          required
        />
        <input
          name="address"
          placeholder="Address"
          value={form.address}
          onChange={handleInputChange}
        />
        <input
          name="phoneNumber"
          placeholder="Phone Number (10 digits)"
          value={form.phoneNumber}
          onChange={handleInputChange}
        />
        <button type="submit">Add Advisor</button>
      </form>
    </div>
  );
}
