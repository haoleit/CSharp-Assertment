
import React, { useState, useEffect } from "react";
import axios from "axios";


function EditTaskModal({
  show,
  onClose,
  onSubmitSuccess,
  taskToEdit,
  TaskStatus,
  navigate,
}) {
  const [editingTask, setEditingTask] = useState(null);
  const [editError, setEditError] = useState(null);


  useEffect(() => {
    if (show && taskToEdit) {
      setEditingTask({
        ...taskToEdit,
     
        dueDate: taskToEdit.dueDate
          ? new Date(taskToEdit.dueDate).toISOString().split("T")[0]
          : "",
      });
      setEditError(null); 
    } else {
      
      setEditingTask(null);
      setEditError(null);
    }
  }, [show, taskToEdit]);

  const handleEditTaskChange = (e) => {
    const { name, value } = e.target;
    setEditingTask((prev) => (prev ? { ...prev, [name]: value } : null));
  };

  const handleUpdateTask = async (e) => {
    e.preventDefault();
    setEditError(null);

    if (!editingTask || !editingTask.title) {
      setEditError("Title is required.");
      return;
    }

    const updateDto = {
      title: editingTask.title,
      description: editingTask.description || null,
      status: parseInt(editingTask.status),
      dueDate: taskToEdit.dueDate,
    };

    try {
      await axios.put(
        `http://localhost:5069/api/Tasks/${editingTask.id}`,
        updateDto,
        {
          withCredentials: true,
        }
      );
      onSubmitSuccess(); 
    } catch (err) {
      console.error("Error updating task:", err);
      setEditError(
        err.response?.data?.title ||
          err.response?.data?.errors?.Title?.[0] ||
          err.response?.data ||
          "Failed to update task. Please try again."
      );
      if (err.response && err.response.status === 401 && navigate) {
        navigate("/login");
      }
    }
  };

  
  if (!show || !editingTask) {
    return null;
  }

  return (
    <div className="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full flex items-center justify-center z-50">
      <div className="relative mx-auto p-5 border w-full max-w-md shadow-lg rounded-md bg-white">
        <h3 className="text-lg font-medium leading-6 text-gray-900 mb-4">
          Edit Task
        </h3>
        <form onSubmit={handleUpdateTask}>
          {/* Title */}
          <div className="mb-4">
            <label
              htmlFor="edit-title"
              className="block text-sm font-medium text-gray-700"
            >
              Title <span className="text-red-500">*</span>
            </label>
            <input
              type="text"
              name="title"
              id="edit-title"
              value={editingTask.title}
              onChange={handleEditTaskChange}
              required
              className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
            />
          </div>
          {/* Description */}
          <div className="mb-4">
            <label
              htmlFor="edit-description"
              className="block text-sm font-medium text-gray-700"
            >
              Description
            </label>
            <textarea
              name="description"
              id="edit-description"
              rows="3"
              value={editingTask.description || ""} 
              onChange={handleEditTaskChange}
              className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
            ></textarea>
          </div>
          {/* Status */}
          <div className="mb-4">
            <label
              htmlFor="edit-status"
              className="block text-sm font-medium text-gray-700"
            >
              Status
            </label>
            <select
              name="status"
              id="edit-status"
              value={editingTask.status}
              onChange={handleEditTaskChange}
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
              htmlFor="edit-dueDate"
              className="block text-sm font-medium text-gray-700"
            >
              Due Date
            </label>
            <input
              type="date"
              name="dueDate"
              id="edit-dueDate"
              value={editingTask.dueDate} 
              onChange={handleEditTaskChange}
              className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
            />
          </div>

          {editError && (
            <p className="text-red-500 text-sm mb-4">{editError}</p>
          )}

          {/* Buttons */}
          <div className="flex justify-end space-x-2">
            <button
              type="button"
              onClick={onClose} 
              className="px-4 py-2 bg-gray-300 text-gray-800 rounded-md hover:bg-gray-400"
            >
              Cancel
            </button>
            <button
              type="submit"
              className="px-4 py-2 bg-blue-500 text-white rounded-md hover:bg-blue-600"
            >
              Save Changes
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default EditTaskModal;
