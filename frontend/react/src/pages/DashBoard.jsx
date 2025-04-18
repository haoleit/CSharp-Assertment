import { useNavigate } from "react-router-dom";
import { useState, useEffect } from "react";
import axios from "axios";
import CreateTaskModal from "../components/CreateTaskModal"; // Import CreateTaskModal
import EditTaskModal from "../components/EditTaskModal"; // Import EditTaskModal

function Dashboard() {
  const navigate = useNavigate();
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [tasks, setTasks] = useState([]);
  const [filteredTasks, setFilteredTasks] = useState([]);
  const [statusFilter, setStatusFilter] = useState("");
  const [sortOrder, setSortOrder] = useState("asc");
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);
  const [showCreateModal, setShowCreateModal] = useState(false); // State to toggle CreateTaskModal
  const [showEditModal, setShowEditModal] = useState(false); // State to toggle EditTaskModal
  const [taskToEdit, setTaskToEdit] = useState(null); // Task to edit in EditTaskModal

  const getStatusString = (statusValue) => {
    switch (statusValue) {
      case 0:
        return "To Do";
      case 1:
        return "In Progress";
      case 2:
        return "Completed";
      default:
        return "Unknown";
    }
  };

  const fetchTasks = async () => {
    setIsLoading(true);
    setError(null);
    try {
      const response = await axios.get("http://localhost:5069/api/Tasks", {
        withCredentials: true,
      });
      const tasksWithDates = response.data.map((task) => ({
        ...task,
        dueDate: task.dueDate ? new Date(task.dueDate) : null,
      }));
      setTasks(tasksWithDates);
      setFilteredTasks(tasksWithDates);
    } catch (err) {
      setError("Failed to load tasks. Please try again later.");
      if (err.response && err.response.status === 401) {
        navigate("/login");
      }
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    axios
      .get("http://localhost:5069/api/Auth/session", { withCredentials: true })
      .then(() => {
        setIsAuthenticated(true);
        fetchTasks();
      })
      .catch(() => {
        setIsAuthenticated(false);
        navigate("/login");
      });
  }, [navigate]);

  useEffect(() => {
    let processedTasks = [...tasks];
    if (statusFilter !== "") {
      processedTasks = processedTasks.filter(
        (task) => task.status === parseInt(statusFilter)
      );
    }
    processedTasks.sort((a, b) => {
      const dateA = a.dueDate
        ? a.dueDate.getTime()
        : sortOrder === "asc"
        ? Infinity
        : -Infinity;
      const dateB = b.dueDate
        ? b.dueDate.getTime()
        : sortOrder === "asc"
        ? Infinity
        : -Infinity;
      return sortOrder === "asc" ? dateA - dateB : dateB - dateA;
    });
    setFilteredTasks(processedTasks);
  }, [tasks, statusFilter, sortOrder]);

  const handleLogout = async () => {
    try {
      const response = await axios.post(
        "http://localhost:5069/api/auth/logout",
        {},
        { withCredentials: true }
      );
      if (response.status === 200) {
        navigate("/login");
      }
    } catch (error) {
      console.error("An error occurred during logout:", error);
    }
  };

  const handleStatusFilter = (statusValue) => {
    setStatusFilter(statusValue);
  };

  const handleSort = (order) => {
    setSortOrder(order);
  };

  const handleEditTask = (task) => {
    setTaskToEdit(task);
    setShowEditModal(true);
  };

  const handleDeleteTask = async (taskId) => {
    if (!window.confirm("Are you sure you want to delete this task?")) {
      return;
    }
    try {
      await axios.delete(`http://localhost:5069/api/Tasks/${taskId}`, {
        withCredentials: true,
      });
      fetchTasks();
    } catch (err) {
      setError("Failed to delete task. Please try again.");
      if (err.response && err.response.status === 401) {
        navigate("/login");
      }
    }
  };

  if (!isAuthenticated) {
    return null;
  }

  return (
    <div className="min-h-screen bg-gray-100 p-8">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl font-bold">My Tasks</h1>
        <div className="flex space-x-4">
          <button
            onClick={() => setShowCreateModal(true)}
            className="bg-green-500 hover:bg-green-600 text-white px-4 py-2 rounded"
          >
            Create New Task
          </button>
          <button
            onClick={handleLogout}
            className="bg-red-500 hover:bg-red-600 text-white px-4 py-2 rounded"
          >
            Logout
          </button>
        </div>
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
            <option value={0}>To Do</option>
            <option value={1}>In Progress</option>
            <option value={2}>Completed</option>
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

      {/* Task List Area */}
      {isLoading && (
        <p className="text-center text-gray-500">Loading tasks...</p>
      )}
      {error && <p className="text-center text-red-500">{error}</p>}
      {!isLoading && !error && (
        <div className="space-y-4">
          {filteredTasks.length === 0 ? (
            <p className="text-center text-gray-500">No tasks found.</p>
          ) : (
            filteredTasks.map((task) => (
              <div
                key={task.id}
                className="bg-white p-4 rounded shadow-md hover:shadow-lg transition duration-200 ease-in-out flex justify-between items-start"
              >
                <div>
                  <h3 className="text-xl font-semibold">{task.title}</h3>
                  <p className="text-gray-700 my-1">
                    {task.description || "No description"}
                  </p>
                  <p
                    className={`text-sm font-medium ${
                      task.status === 2
                        ? "text-green-600"
                        : task.status === 1
                        ? "text-yellow-600"
                        : "text-blue-600"
                    }`}
                  >
                    Status: {getStatusString(task.status)}
                  </p>
                  <p className="text-sm text-gray-500">
                    Due:{" "}
                    {task.dueDate
                      ? task.dueDate.toLocaleDateString()
                      : "No due date"}
                  </p>
                </div>

                <div className="mt-3 flex space-x-2">
                  <button
                    onClick={() => handleEditTask(task)}
                    className="text-sm bg-yellow-500 hover:bg-yellow-600 text-white px-3 py-1 rounded"
                  >
                    Edit
                  </button>
                  <button
                    onClick={() => handleDeleteTask(task.id)}
                    className="text-sm bg-red-500 hover:bg-red-600 text-white px-3 py-1 rounded"
                  >
                    Delete
                  </button>
                </div>
              </div>
            ))
          )}
        </div>
      )}

      {/* Create Task Modal */}
      {showCreateModal && (
        <CreateTaskModal
          show={showCreateModal}
          onClose={() => setShowCreateModal(false)}
          onSubmitSuccess={() => {
            setShowCreateModal(false);
            fetchTasks();
          }}
          TaskStatus={{ ToDo: 0, InProgress: 1, Completed: 2 }} // Pass status enum
          navigate={navigate}
        />
      )}

      {/* Edit Task Modal */}
      {showEditModal && taskToEdit && (
        <EditTaskModal
          show={showEditModal}
          onClose={() => setShowEditModal(false)}
          onSubmitSuccess={() => {
            setShowEditModal(false);
            fetchTasks();
          }}
          taskToEdit={taskToEdit}
          TaskStatus={{ ToDo: 0, InProgress: 1, Completed: 2 }}
          navigate={navigate}
        />
      )}
    </div>
  );
}

export default Dashboard;
