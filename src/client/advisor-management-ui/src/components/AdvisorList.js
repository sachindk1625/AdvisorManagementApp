export default function AdvisorList({ advisors, handleDelete }) {
  return (
    <div>
      <h1>Advisors</h1>
      <table>
        <thead>
          <tr>
            <th>FullName</th>
            <th>SIN</th>
            <th>Phone Number</th>
            <th>Health Status</th>
            <th>Action</th>
          </tr>
        </thead>
        <tbody>
          {advisors.map((advisor) => {
            return (<tr key={advisor.advisorId}>
                <td>{advisor.fullName}</td>
                <td>{advisor.sin}</td>
                <td>{advisor.phoneNumber}</td>
                <td>{advisor.healthStatus}</td>
                <td>
                  <button onClick={() => handleDelete(advisor.advisorId)}>
                    Delete
                  </button>
                </td>
              </tr>)
          })}
        </tbody>
      </table>
    </div>
  );
}
