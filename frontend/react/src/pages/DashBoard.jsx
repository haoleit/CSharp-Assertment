import { useNavigate } from "react-router-dom";
import { useState, useEffect } from "react";

function Dashboard() {
  const navigate = useNavigate();

  // Dữ liệu giả cho các task (giống như đã làm ở trên)
  const [tasks, setTasks] = useState([]);
  const [filteredTasks, setFilteredTasks] = useState([]);
  const [statusFilter, setStatusFilter] = useState("");
  const [sortOrder, setSortOrder] = useState("asc");

  useEffect(() => {
    const data = [
      {
        id: 1,
        title: "Complete React Project",
        description: "Finish building the React task manager project.",
        status: "in progress",
        dueDate: new Date(2025, 4, 5),
      },
      {
        id: 2,
        title: "Buy groceries",
        description: "Get fruits, vegetables, and milk.",
        status: "completed",
        dueDate: new Date(2025, 3, 20),
      },
      {
        id: 3,
        title: "Prepare for interview",
        description: "Review algorithm and data structures.",
        status: "to-do",
        dueDate: new Date(2025, 4, 10),
      },
      {
        id: 4,
        title: "Clean the house",
        description: "Vacuum and mop the floors.",
        status: "to-do",
        dueDate: new Date(2025, 4, 2),
      },
      {
        id: 5,
        title: "Fix bugs in code",
        description: "Fix bugs reported by QA team.",
        status: "in progress",
        dueDate: new Date(2025, 3, 30),
      },
    ];
    setTasks(data);
    setFilteredTasks(data);
  }, []);

  // Xử lý logout
  const handleLogout = () => {
    localStorage.removeItem("token"); // Xoá token
    navigate("/login"); // Điều hướng về trang đăng nhập
  };

  // Xử lý filtering
  const handleStatusFilter = (status) => {
    setStatusFilter(status);
    const filtered = tasks.filter((task) =>
      status ? task.status === status : true
    );
    setFilteredTasks(filtered);
  };

  // Xử lý sorting
  const handleSort = (order) => {
    setSortOrder(order);
    const sorted = [...filteredTasks].sort((a, b) => {
      if (order === "asc") {
        return a.dueDate - b.dueDate;
      } else {
        return b.dueDate - a.dueDate;
      }
    });
    setFilteredTasks(sorted);
  };

  return (
    <div className="min-h-screen bg-gray-100 p-8">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl font-bold">My Tasks</h1>
        <button
          onClick={handleLogout}
          className="bg-red-500 hover:bg-red-600 text-white px-4 py-2 rounded"
        >
          Logout
        </button>
      </div>

      {/* Filtering */}
      <div className="mb-6 flex items-center space-x-6">
        <div className="flex items-center">
          <label className="mr-2 text-lg font-semibold">
            Filter by status:
          </label>
          <select
            value={statusFilter}
            onChange={(e) => handleStatusFilter(e.target.value)}
            className="px-4 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 transition duration-300 ease-in-out hover:bg-blue-50"
          >
            <option value="">All</option>
            <option value="to-do">To Do</option>
            <option value="in progress">In Progress</option>
            <option value="completed">Completed</option>
          </select>
        </div>

        {/* Sorting */}
        <div className="flex items-center">
          <label className="mr-2 text-lg font-semibold">
            Sort by due date:
          </label>
          <button
            onClick={() => handleSort("asc")}
            className={`px-4 py-2 rounded-md text-white transition duration-300 ease-in-out ${
              sortOrder === "asc"
                ? "bg-blue-500 hover:bg-blue-600"
                : "bg-gray-300 hover:bg-gray-400"
            }`}
          >
            Ascending
          </button>
          <button
            onClick={() => handleSort("desc")}
            className={`ml-4 px-4 py-2 rounded-md text-white transition duration-300 ease-in-out ${
              sortOrder === "desc"
                ? "bg-blue-500 hover:bg-blue-600"
                : "bg-gray-300 hover:bg-gray-400"
            }`}
          >
            Descending
          </button>
        </div>
      </div>

      {/* Task List */}
      <div className="space-y-4">
        {filteredTasks.map((task) => (
          <div
            key={task.id}
            className="bg-white p-4 rounded shadow-md hover:shadow-lg transition duration-200 ease-in-out"
          >
            <h3 className="text-xl font-semibold">{task.title}</h3>
            <p>{task.description}</p>
            <p
              className={`text-sm ${
                task.status === "completed"
                  ? "text-green-500"
                  : task.status === "in progress"
                  ? "text-yellow-500"
                  : "text-gray-500"
              }`}
            >
              Status: {task.status}
            </p>
            <p className="text-sm text-gray-400">
              Due: {task.dueDate.toLocaleDateString()}
            </p>
          </div>
        ))}
      </div>
    </div>
  );
}

export default Dashboard;
