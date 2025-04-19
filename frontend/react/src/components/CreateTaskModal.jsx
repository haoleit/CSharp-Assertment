// frontend/react/src/components/CreateTaskModal.jsx
import React, { useState, useEffect } from "react";
import axios from "axios";

// Receive TaskStatus enum/mapping from props
function CreateTaskModal({
  show,
  onClose,
  onSubmitSuccess,
  TaskStatus,
  navigate,
}) {
  const [newTask, setNewTask] = useState({
    title: "",
    description: "",
    status: TaskStatus.ToDo, // Use passed TaskStatus
    dueDate: "",
  });
  const [createError, setCreateError] = useState(null);

  // Reset form when the modal is shown or closed
  useEffect(() => {
    if (show) {
      setNewTask({
        title: "",
        description: "",
        status: TaskStatus.ToDo,
        dueDate: "",
      });
      setCreateError(null);
    }
  }, [show, TaskStatus.ToDo]);

  const handleNewTaskChange = (e) => {
    const { name, value } = e.target;
    setNewTask((prev) => ({ ...prev, [name]: value }));
  };

  const handleCreateTask = async (e) => {
    e.preventDefault();
    setCreateError(null);

    if (!newTask.title) {
      setCreateError("Title is required.");
      return;
    }

    const taskToSubmit = {
      title: newTask.title,
      description: newTask.description || null,
      status: parseInt(newTask.status),
      dueDate: newTask.dueDate ? new Date(newTask.dueDate) : new Date() + 3,
    };

    try {
      await axios.post("http://localhost:5069/api/Tasks", taskToSubmit, {
        withCredentials: true,
      });
      onSubmitSuccess(); // Notify parent component (Dashboard)
    } catch (err) {
      console.error("Error creating task:", err);
      setCreateError(
        err.response?.data?.title ||
          err.response?.data?.errors?.Title?.[0] ||
          err.response?.data ||
          "Failed to create task. Please try again."
      );
      if (err.response && err.response.status === 401 && navigate) {
        navigate("/login");
      }
    }
  };

  if (!show) {
    return null;
  }

  return (
    <div className="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full flex items-center justify-center z-50">
      <div className="relative mx-auto p-5 border w-full max-w-md shadow-lg rounded-md bg-white">
        <h3 className="text-lg font-medium leading-6 text-gray-900 mb-4">
          Create New Task
        </h3>
        <form onSubmit={handleCreateTask}>
          {/* Title */}
          <div className="mb-4">
            <label
              htmlFor="title"
              className="block text-sm font-medium text-gray-700"
            >
              Title <span className="text-red-500">*</span>
            </label>
            <input
              type="text"
              name="title"
              id="title"
              value={newTask.title}
              onChange={handleNewTaskChange}
              required
              className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
            />
          </div>
          {/* Description */}
          <div className="mb-4">
            <label
              htmlFor="description"
              className="block text-sm font-medium text-gray-700"
            >
              Description
            </label>
            <textarea
              name="description"
              id="description"
              rows="3"
              value={newTask.description}
              onChange={handleNewTaskChange}
              className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
            ></textarea>
          </div>
          {/* Status */}
          <div className="mb-4">
            <label
              htmlFor="status"
              className="block text-sm font-medium text-gray-700"
            >
              Status
            </label>
            <select
              name="status"
              id="status"
              value={newTask.status}
              onChange={handleNewTaskChange}
              className="mt-1 block w-full px-3 py-2 border border-gray-300 bg-white rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
            >
              <option value={TaskStatus.ToDo}>To Do</option>
              <option value={TaskStatus.InProgress}>In Progress</option>
              <option value={TaskStatus.Completed}>Completed</option>
            </select>
          </div>
          {/* Due Date */}
          <div className="mb-4">
            <label
              htmlFor="dueDate"
              className="block text-sm font-medium text-gray-700"
            >
              Due Date
            </label>
            <input
              type="date"
              name="dueDate"
              id="dueDate"
              value={newTask.dueDate}
              onChange={handleNewTaskChange}
              className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
            />
          </div>

          {createError && (
            <p className="text-red-500 text-sm mb-4">{createError}</p>
          )}

          {/* Buttons */}
          <div className="flex justify-end space-x-2">
            <button
              type="button"
              onClick={onClose} // Use onClose prop
              className="px-4 py-2 bg-gray-300 text-gray-800 rounded-md hover:bg-gray-400"
            >
              Cancel
            </button>
            <button
              type="submit"
              className="px-4 py-2 bg-green-500 text-white rounded-md hover:bg-green-600"
            >
              Create Task
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default CreateTaskModal;
